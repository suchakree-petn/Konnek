using Unity.Netcode;
using UnityEngine;

public partial class MainGameManager
{
    public void Server_EndTurn(ref MainGameContext context)
    {
        if (CommandQueue.commandsQueue.Count > 0)
        {
            context.currentState = MainGameState.Idle;
            context.GetCurrentPlayerContext().IsPlayerTurn = false;
        }
        Command endTurnCommand = new EndTurnCommand();
        CommandQueue.AddCommand(endTurnCommand);
        CommandQueue.TryExecuteCommandsServerRpc();
    }
    public void EndTurnButton()
    {
        EndTurnButtonServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void EndTurnButtonServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        if (!MainGameContext.IsOwnerTurn(clientId)) return;
        Server_EndTurn(ref MainGameContext);

    }
}