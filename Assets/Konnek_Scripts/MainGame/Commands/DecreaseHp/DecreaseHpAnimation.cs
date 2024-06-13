using System;

public class DecreaseHpAnimation : Command
{
    private ulong clientId;
    private int from;
    private int to;
    public static Action OnHpDecreasedAnimationFinish;

    public DecreaseHpAnimation(ulong clientId, int from, int to)
    {
        this.clientId = clientId;
        this.from = from;
        this.to = to;
    }

    public override void Execute()
    {
        OnHpDecreasedAnimationFinish += OnFinishExecute;
        KonnekUIManager.Instance.HpDecreasedAnimation(clientId, from, to);

    }

    private void OnFinishExecute()
    {
        OnHpDecreasedAnimationFinish -= OnFinishExecute;
        base.Execute();
    }
}
