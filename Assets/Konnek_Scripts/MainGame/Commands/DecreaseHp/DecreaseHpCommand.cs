using System;
using UnityEngine;

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
        int from = mainGameContext.GetPlayerHp(clientId);
        int currentHp = from - amount;
        if (currentHp < 0)
        {
            currentHp = 0;
        }

        mainGameContext.SetPlayerHp(clientId, currentHp);
        KonnekUIManager.Instance.HpDecreasedAnimation_ClientRpc(clientId, from, currentHp);

        base.Execute();

    }
}
