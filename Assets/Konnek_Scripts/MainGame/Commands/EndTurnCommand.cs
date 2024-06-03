using UnityEngine;

public class EndTurnCommand : Command
{
    public EndTurnCommand()
    {
    }

    public override void Execute()
    {
        MainGameContext context = MainGameManager.Instance.MainGameContext;
        PlayerContext currentPlayer = context.GetCurrentPlayerContext();

        context.currentState = currentPlayer.PlayerOrderIndex == 1 ? MainGameState.Player_1_End_Turn : MainGameState.Player_2_End_Turn;

        context.currentTurnDuration = context.TurnDuration;
        OnComplete(() =>
        {
            Debug.Log("Finish end turn command");
        });
        base.Execute();
        Debug.Log("End turn!");
    }

}
