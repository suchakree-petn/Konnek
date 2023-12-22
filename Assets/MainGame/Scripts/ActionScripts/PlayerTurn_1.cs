using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class PlayerTurn_1 : ActionNode
{
    public PlayerContext playerContext;
    protected override void OnStart()
    {
        InGameContext inGameContext = blackboard.inGameContext;
        PlayerContext player_1 = inGameContext.playerContext_1;
        playerContext = player_1;

        MainGameManager.OnStartTurn_1?.Invoke(inGameContext);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}
