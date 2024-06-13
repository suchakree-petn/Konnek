using UnityEngine;

public class EndTurnCommand : Command
{
    int currentPlayerIndex;
    public EndTurnCommand(int currentPlayerIndex)
    {
        this.currentPlayerIndex = currentPlayerIndex;
    }

    public override void Execute()
    {
        MainGameManager mainGameManager = MainGameManager.Instance;
        MainGameContext mainGameContext = mainGameManager.MainGameContext;
        PlayerContext playerContext = mainGameContext.PlayerContextByPlayerOrder[currentPlayerIndex];

        // check is player play piece if not then play it
        if (!playerContext.IsPlayedPiece)
        {
            KonnekUIManager.Instance.PlayButton(playerContext.GetClientId());
        }
        playerContext.IsPlayedPiece = false;

        // set timer and current game state to "Player [?] end turn"
        mainGameContext.currentTurnDuration = mainGameContext.TurnDuration;
        MainGameState endTurn = currentPlayerIndex == 1 ? MainGameState.Player_1_End_Turn : MainGameState.Player_2_End_Turn;
        mainGameManager.SetCurrentGameState(endTurn);

        // check is all command execute before end turn
        CommandQueue commandsQueue = mainGameManager.CommandQueue;
        CommandQueue animationQueue = mainGameManager.AnimationQueue;
        int commandsQueueCount = commandsQueue.commandsQueue.Count;
        int animationQueueCount = animationQueue.commandsQueue.Count;
        if (commandsQueueCount > 0 || animationQueueCount > 0)
        {
            mainGameManager.SetCurrentGameState(MainGameState.Idle);

            MainGameState startTurn = currentPlayerIndex == 1 ? MainGameState.Player_2_Start_Turn : MainGameState.Player_1_Start_Turn;

            CommandQueue _commandQueue = commandsQueueCount > animationQueueCount ? commandsQueue : animationQueue;
            _commandQueue.GetLastCommand().OnComplete(() => mainGameManager.SetCurrentGameState(startTurn));

            playerContext.IsPlayerTurn = false;
        }
        base.Execute();
    }

}
