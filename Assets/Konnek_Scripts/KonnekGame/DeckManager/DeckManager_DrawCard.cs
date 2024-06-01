using Unity.Netcode;
using UnityEngine;
public partial class DeckManager
{
    public void DrawCardFromPlayerId(string cardId, ulong playerId = default)
    {
        if (playerId == default)
        {
            // playerId = PlayerManager.GetOwnerPlayerId();
        }
        Transform card = SpawnCard(cardId, CardState.InHand);
        DrawAnimation(card, MainGameManager.Instance.mainGameContext.GetPlayerIndexByPlayerId(playerId));
        OnDrawCard?.Invoke();
    }
    [ClientRpc]
    public void DrawCardFromTopDeckClientRpc(ulong playerId, ulong reqClientId, int playerIndex)
    {
        if (reqClientId != NetworkManager.Singleton.LocalClientId) return;

        Deck deck;
        // if (playerId == "default")
        // {
        //     playerId = PlayerManager.GetOwnerPlayerId();
        // }
        deck = decks[playerId];

        Transform card = SpawnCard(deck.GetCardFromTopDeck(), CardState.InHand);
        DrawAnimation(card, playerIndex);
        OnDrawCard?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DrawCardFromTopDeckServerRpc(ulong playerId = default, ServerRpcParams rpcParams = default)
    {
        if (playerId == default)
        {
            // playerId = PlayerManager.GetOwnerPlayerId();
        }
        ulong reqClientId = rpcParams.Receive.SenderClientId;
        MainGameContext mainGameContext = MainGameManager.Instance.mainGameContext;
        if (mainGameContext.CanDraw(reqClientId))
        {
            mainGameContext.GetPlayerContextByClientId(reqClientId).drawCardQuota--;
            int playerIndex = MainGameManager.Instance.mainGameContext.GetPlayerIndexByClientId(reqClientId);
            DrawCardFromTopDeckClientRpc(playerId, reqClientId, playerIndex);
        }
    }
}
