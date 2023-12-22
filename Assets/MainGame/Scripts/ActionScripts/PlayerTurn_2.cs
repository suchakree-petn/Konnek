using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class PlayerTurn_2 : ActionNode
{
    public PlayerContext playerContext;
    protected override void OnStart()
    {
        PlayerContext player_2 = blackboard.inGameContext.playerContext_2;
        playerContext = player_2;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}
