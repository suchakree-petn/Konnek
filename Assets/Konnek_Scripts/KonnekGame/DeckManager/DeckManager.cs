using System;
using System.Collections.Generic;
using UnityEngine;

public partial class DeckManager : NetworkSingleton<DeckManager>
{
    public Action OnDrawCard;
    public Dictionary<ulong, Deck> DeckDict = new();
    public List<Deck> Decks = new();
    [SerializeField] private Transform card_prf;

    public void InitDecks(MainGameContext mainGameContext)
    {
        DeckDict[mainGameContext.PlayerContextByPlayerOrder[1].GetClientId()] = Decks[0];
        DeckDict[mainGameContext.PlayerContextByPlayerOrder[2].GetClientId()] = Decks[1];
    }

    private Transform SpawnCard(ulong cardId, CardState cardState = CardState.Default)
    {
        // if (cardId is null)
        // {
        //     Debug.Log("Card is null");
        //     return null;
        // }
        Card card = Card.Cache[cardId];
        Transform card_GO = Instantiate(card_prf, deckAnimationConfig.deckTransform_1.parent);
        CardHolder cardHolder = card_GO.GetComponent<CardHolder>();
        cardHolder.InitCardData(card);
        cardHolder.CardState = cardState;
        return card_GO;
    }
    protected override void InitAfterAwake()
    {
    }
    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        MainGameManager.Instance.OnGameStart += InitDecks;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsServer) return;
        MainGameManager.Instance.OnGameStart -= InitDecks;
    }
    private void OnEnable()
    {
        DeckManagerPointerEventHandler.OnDeckClick += () => DrawCardFromTopDeckServerRpc();
        MainGameManager.Instance.OnStartTurn_Player_1 += (MainGameContext context) =>
        {
            MainGameManager.Instance.MainGameContext.PlayerContextByPlayerOrder[1].DrawCardQuota = 1;
        };
        MainGameManager.Instance.OnStartTurn_Player_2 += (MainGameContext context) =>
        {
            MainGameManager.Instance.MainGameContext.PlayerContextByPlayerOrder[2].DrawCardQuota = 1;
        };
    }
}
