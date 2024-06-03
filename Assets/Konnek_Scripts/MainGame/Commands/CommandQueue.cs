using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class CommandQueue
{
    public readonly Queue<Command> commandsQueue;
    [SerializeField] private bool isExecuting;
    public bool IsExecuting => isExecuting;

    public CommandQueue()
    {
        commandsQueue = new();
    }
    public void AddCommand(Command command)
    {
        command.OnComplete(OnFinishExecute);
        commandsQueue.Enqueue(command);
        TryExecuteCommandsServerRpc();
        Debug.Log($"Command queue size: {commandsQueue.Count}");
    }
    [ServerRpc(RequireOwnership = false)]
    public void TryExecuteCommandsServerRpc()
    {
        if (isExecuting) return;
        if (commandsQueue.Count > 0)
        {
            isExecuting = true;
            // Debug.Log($"Command queue size: {commandsQueue.Count}");
            Command command = commandsQueue.Dequeue();
            if (commandsQueue.Count > 0 && MainGameManager.Instance.MainGameContext.currentState == MainGameState.Idle)
            {
                command.OnComplete(TryExecuteCommandsServerRpc);
            }
            command.Execute();
        }
    }
    public void OnFinishExecute()
    {
        isExecuting = false;
        TryExecuteCommandsServerRpc();
    }
}
