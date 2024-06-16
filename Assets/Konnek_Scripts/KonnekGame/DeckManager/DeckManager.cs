using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class DeckManager : NetworkSingleton<DeckManager>
{
    public Action OnDrawCard;
    public Dictionary<ulong, Deck> DeckDict = new();
    public List<Deck> Decks = new();
    [SerializeField] private Transform card_prf;

    public Transform PlayerDeckTransform;
    public Transform OpponentDeckTransform;

    public Dictionary<ulong, Transform> PlayerDeckTransformDict = new();


    protected override void InitAfterAwake()
    {


    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        MainGameContext mainGameContext = MainGameManager.Instance.MainGameContext;
        InitDecks(mainGameContext);
        InitPlayerDeckTransform_ClientRpc(
            mainGameContext.GetPlayerContextByIndex(1).GetClientId()
            , mainGameContext.GetPlayerContextByIndex(2).GetClientId());

    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
    }

    [ClientRpc]
    private void InitPlayerDeckTransform_ClientRpc(ulong clientId_1, ulong clientId_2)
    {
        if (NetworkManager.LocalClientId != clientId_1)
        {
            PlayerDeckTransformDict.Add(clientId_2, PlayerDeckTransform);
            PlayerDeckTransformDict.Add(clientId_1, OpponentDeckTransform);

        }
        else
        {
            PlayerDeckTransformDict.Add(clientId_1, PlayerDeckTransform);
            PlayerDeckTransformDict.Add(clientId_2, OpponentDeckTransform);
        }
    }

    public Transform GetDeckTransformByDeckIndex(int index)
    {
        return index == 1 ? PlayerDeckTransform : OpponentDeckTransform;
    }

    public void InitDecks(MainGameContext mainGameContext)
    {
        DeckDict[mainGameContext.PlayerContextByPlayerOrder[1].GetClientId()] = Decks[0];
        DeckDict[mainGameContext.PlayerContextByPlayerOrder[2].GetClientId()] = Decks[1];
    }


    public void SpawnCard(ulong cardId, ulong reqClientId, uint cardInstanceId, DrawCardAnimation animation)
    {
        Transform card = SpawnCardGO(cardId, reqClientId, cardInstanceId, CardState.InHand);

        if (reqClientId != NetworkManager.LocalClientId) // If not own this card. Cant see card's face
        {
            card.GetComponent<CardHolder>().SetCardFaceDown();
        }

        DrawAnimation(card, reqClientId, animation);
        OnDrawCard?.Invoke();

    }

    private Transform SpawnCardGO(ulong cardId, ulong drawClientId, uint cardInstanceId, CardState cardState = CardState.Default)
    {
        Card card = Card.Cache[cardId];
        Transform card_GO = Instantiate(card_prf, PlayerDeckTransformDict[drawClientId].parent);
        CardHolder cardHolder = card_GO.GetComponent<CardHolder>();
        cardHolder.InitCardData(card, cardInstanceId);
        cardHolder.CardState = cardState;
        cardHolder.OwnerClientId = drawClientId;
        return card_GO;
    }

    [ContextMenu("Shuffle Deck 1")]
    public void ShuffleDeck1()
    {
        Decks[0].ShuffleDeck();
    }

    [ContextMenu("Shuffle Deck 2")]
    public void ShuffleDeck2()
    {
        Decks[1].ShuffleDeck();
    }
}
