using System;

public class DecreaseHpCommand : Command
{
    private ulong clientId;
    private int amount;

    public DecreaseHpCommand(ulong clientId, int amount)
    {
        this.clientId = clientId;
        this.amount = amount;
    }

    public override void Execute()
    {
        MainGameContext mainGameContext = MainGameManager.Instance.MainGameContext;
        int currentHp = mainGameContext.GetPlayerHp(clientId);
        currentHp -= amount;
        if (currentHp < 0)
        {
            currentHp = 0;
        }
        mainGameContext.SetPlayerHp(clientId, currentHp);
        base.Execute();

    }
}