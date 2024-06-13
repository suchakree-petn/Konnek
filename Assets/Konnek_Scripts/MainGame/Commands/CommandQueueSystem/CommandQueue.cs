using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class CommandQueue
{
    public readonly Queue<Command> commandsQueue;
    [SerializeField] private Command currentExecutingCommand;
    public string CurrentExecutingCommand => currentExecutingCommand.GetCommandName();
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
        TryExecuteCommands();
        // Debug.Log($"Command queue size: {commandsQueue.Count}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryExecuteCommands()
    {
        if (isExecuting) return;
        if (commandsQueue.Count > 0)
        {
            isExecuting = true;
            // Debug.Log($"Command queue size: {commandsQueue.Count}");
            Command command = commandsQueue.Dequeue();
            currentExecutingCommand = command;
            if (commandsQueue.Count > 0 && MainGameManager.Instance.GetCurrentGameState() == MainGameState.Idle)
            {
                command.OnComplete(TryExecuteCommands);
            }
            command.Execute();
        }
    }

    public void OnFinishExecute()
    {
        isExecuting = false;
        TryExecuteCommands();
    }

    public Command GetLastCommand()
    {
        return commandsQueue.Peek();
    }
}
