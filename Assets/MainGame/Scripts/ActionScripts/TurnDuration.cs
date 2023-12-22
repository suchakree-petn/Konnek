using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    public class TurnDuration : ActionNode
    {
        public float duration = 1;
        float startTime;

        protected override void OnStart()
        {
            duration = blackboard.inGameContext.inGameSetting.turnDuration;
            startTime = Time.time;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            bool isEndTurn = false;
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isEndTurn = true;
            }
            if (Time.time - startTime > duration || isEndTurn)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}
