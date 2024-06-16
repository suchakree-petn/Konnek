using System;
using UnityEngine;

public class StartTurnAnimation : Command
{
    public static Action OnStartTurnAnimationFinish;
    public const string START_TURN_ANIMATION_NAME = "Start turn animation command";


    public StartTurnAnimation()
    {
        name = START_TURN_ANIMATION_NAME;
    }

    public override void Execute()
    {
        OnStartTurnAnimationFinish += HandleOnEndTurnAnimationFinish;

        KonnekUIManager.Instance.ShowStartPlayerTurnUI();
    }

    private void HandleOnEndTurnAnimationFinish()
    {
        OnStartTurnAnimationFinish -= HandleOnEndTurnAnimationFinish;
        base.Execute();
    }


}
