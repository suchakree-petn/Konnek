using System;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DecreaseHPCommand : Command
{
    private ulong clientId;
    private int amount;
    public static Action OnHpDecreasedAnimationFinish;

    public DecreaseHPCommand(ulong clientId, int amount)
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
        OnHpDecreasedAnimationFinish += OnFinishExecute;

        KonnekManager.Instance.HpDecreasedAnimation_ClientRpc(clientId);

    }
  
    private void OnFinishExecute()
    {
        OnHpDecreasedAnimationFinish -= OnFinishExecute;
        base.Execute();
    }
}
