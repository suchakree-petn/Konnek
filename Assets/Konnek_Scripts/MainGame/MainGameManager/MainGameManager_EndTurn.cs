using Unity.Netcode;
using UnityEngine;

public partial class MainGameManager
{
    public void Server_EndTurn(ref MainGameContext context)
    {
        if (commandQueue.commandsQueue.Count > 0)
        {
            context.currentState = MainGameState.Idle;
            context.GetCurrentPlayerContext().isPlayerTurn = false;
        }
        Command endTurnCommand = new EndTurnCommand();
        commandQueue.AddCommand(endTurnCommand);
        commandQueue.TryExecuteCommandsServerRpc();
    }
    public void EndTurnButton()
    {
        EndTurnButtonServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void EndTurnButtonServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log(clientId);
        if (!mainGameContext.IsOwnerTurn(clientId)) return;
        Server_EndTurn(ref mainGameContext);

    }
}