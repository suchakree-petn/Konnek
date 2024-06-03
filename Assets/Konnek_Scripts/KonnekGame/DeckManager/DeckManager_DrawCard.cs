using Unity.Netcode;
using UnityEngine;
public partial class DeckManager
{
    public void DrawCardByClientId(ulong cardId, ulong clientId)
    {
        Transform card = SpawnCard(cardId, CardState.InHand);
        DrawAnimation(card, MainGameManager.Instance.MainGameContext.GetPlayerIndexByClientId(clientId));
        OnDrawCard?.Invoke();
    }
    [ClientRpc]
    public void SpawnCardClientRpc(ulong cardId, ulong reqClientId, int playerIndex)
    {
        if (reqClientId != NetworkManager.LocalClientId) return;

        Transform card = SpawnCard(cardId, CardState.InHand);
        DrawAnimation(card, playerIndex);
        OnDrawCard?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DrawCardFromTopDeckServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong reqClientId = rpcParams.Receive.SenderClientId;
        Debug.Log("Request on " + reqClientId);
        MainGameContext mainGameContext = MainGameManager.Instance.MainGameContext;
        if (mainGameContext.CanDraw(reqClientId))
        {
            mainGameContext.GetPlayerContextByClientId(reqClientId).DrawCardQuota--;
            int playerIndex = MainGameManager.Instance.MainGameContext.GetPlayerIndexByClientId(reqClientId);
            Deck deck = DeckDict[reqClientId];

            SpawnCardClientRpc(deck.GetCardFromTopDeck(), reqClientId, playerIndex);
        }
    }
}
