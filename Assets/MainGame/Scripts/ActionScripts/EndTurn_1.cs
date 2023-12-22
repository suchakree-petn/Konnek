using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class EndTurn_1 : ActionNode
{
    protected override void OnStart()
    {
        InGameContext inGameContext = blackboard.inGameContext;
        inGameContext.currentPlayer = inGameContext.playerContext_2;

        MainGameManager.OnEndTurn_1?.Invoke(inGameContext);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}
