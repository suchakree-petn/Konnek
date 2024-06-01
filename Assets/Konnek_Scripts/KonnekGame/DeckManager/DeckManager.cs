using System;
using System.Collections.Generic;
using UnityEngine;

public partial class DeckManager : NetworkSingleton<DeckManager>
{
    public Action OnDrawCard;
    public Dictionary<ulong, Deck> decks = new();
    public List<Deck> Decks = new();

    public void InitDecks()
    {
        // decks[PlayerManager.GetPlayerId(1)] = Decks[0];
        // decks[PlayerManager.GetPlayerId(2)] = Decks[1];
    }

    private Transform SpawnCard(string cardId, CardState cardState = CardState.Default)
    {
        if (cardId is null)
        {
            Debug.Log("Card is null");
            return null;
        }
        Card card = Card.Cache[cardId];
        Transform card_GO = Instantiate(card.card_prf, deckAnimationConfig.deckTransform_1.parent);
        CardHolder cardHolder = card_GO.GetComponent<CardHolder>();
        cardHolder.InitCardData(card);
        cardHolder.cardState = cardState;
        return card_GO;
    }
    protected override void InitAfterAwake()
    {
        InitDecks();
    }

    private void OnEnable()
    {
        DeckManagerPointerEventHandler.OnDeckClick += () => DrawCardFromTopDeckServerRpc();
        MainGameManager.OnStartTurn_Player_1 += (ref MainGameContext context) =>
        {
            context.GetCurrentPlayerContext().drawCardQuota = 1;
        };
        MainGameManager.OnStartTurn_Player_2 += (ref MainGameContext context) =>
        {
            context.GetCurrentPlayerContext().drawCardQuota = 1;
        };
    }
}
