using Unity.Netcode;
using UnityEngine;

public partial class KonnekManager
{
    public PlayerActions playerActions;

    
    [ServerRpc(RequireOwnership = false)]
    public void RequestMoveColumnServerRpc(int column, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log($"Try set to {column}");
        if (MainGameManager.Instance.mainGameContext.IsOwnerTurn(clientId))
        {
            SelectedColumn = column;
        }
        else
        {
            Debug.Log("Not owner's turn");
        }
    }
}
