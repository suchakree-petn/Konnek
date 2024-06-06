using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public partial class DeckManager
{
    // public void DrawCardByClientId(ulong cardId, ulong clientId)
    // {
    //     Transform card = SpawnCard(cardId, CardState.InHand);
    //     DrawAnimation(card, MainGameManager.Instance.MainGameContext.GetPlayerIndexByClientId(clientId));
    //     OnDrawCard?.Invoke();
    // }


    [ServerRpc(RequireOwnership = false)]
    public void DrawCardFromTopDeckServerRpc(ulong clientId, int amount = 1)
    {
        Deck deck = DeckDict[clientId];
        List<ulong> cardIds = new();
        for (int i = 0; i < amount; i++)
        {
            ulong cardId = deck.GetCardFromTopDeck();
            cardIds.Add(cardId);
        }

        foreach (ulong cardId in cardIds)
        {
            if (cardId == 0) continue; // No card left in deck

            DrawCardAnimation_ClientRpc(clientId, cardId);

            Command command = new DrawCardCommand(clientId, cardId);
            MainGameManager.Instance.AddCommand(command);
        }
    }

    [ClientRpc]
    public void DrawCardAnimation_ClientRpc(ulong clientId, ulong cardId)
    {
        Command animationCommand = new DrawCardAnimation(clientId, cardId);
        MainGameManager.Instance.AddAnimationCommand(animationCommand);
    }
}
