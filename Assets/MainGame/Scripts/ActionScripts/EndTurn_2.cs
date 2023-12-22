using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class EndTurn_2 : ActionNode
{
    protected override void OnStart()
    {
        InGameContext inGameContext = blackboard.inGameContext;
        inGameContext.currentPlayer = inGameContext.playerContext_1;

        MainGameManager.OnEndTurn_2?.Invoke(inGameContext);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}
