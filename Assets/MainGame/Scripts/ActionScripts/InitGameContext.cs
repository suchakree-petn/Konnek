using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class InitGameContext : ActionNode
{
    protected override void OnStart()
    {
        blackboard.inGameContext = MainGameManager.Instance.inGameContext;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}
