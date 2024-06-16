using System;
using Unity.Netcode;
using UnityEngine;

public class RandomColumnAnimation : Command
{
    public Action OnRandomColumnAnimationFinish;
    int fixedResult;

    public RandomColumnAnimation(int fixedResult = RandomColumnCommand.FIXED_RESULT_REF)
    {
        this.fixedResult = fixedResult;
    }

    public override void Execute()
    {
        OnRandomColumnAnimationFinish += HandleOnAnimationFinish;
        KonnekUIManager.Instance.SpinWheelAnimation(this, fixedResult);
    }

    private void HandleOnAnimationFinish()
    {
        OnRandomColumnAnimationFinish -= HandleOnAnimationFinish;
        base.Execute();
    }
}
