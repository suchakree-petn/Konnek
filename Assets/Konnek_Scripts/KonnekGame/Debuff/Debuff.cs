using UnityEngine;
using System;

[Serializable]
public abstract class Debuff
{
    public uint InstanceId;
    public abstract DebuffType Type { get; }
    public int Damage;
    public ulong OpponentClientId;
    public ulong UserClientId;
    public int Duration;


    public Debuff(int damage, ulong userClientId, int duration)
    {
        InstanceId = DebuffManager.GetDebuffInstanceId();
        Damage = damage;
        UserClientId = userClientId;
        ulong opponentClientId = MainGameManager.Instance.MainGameContext.GetOpponentPlayerContext().GetClientId();
        OpponentClientId = opponentClientId;
        Duration = duration;
    }

    public abstract void Proc();

    public abstract void OnAddDebuff();
    public abstract void OnRemoveDebuff();
}

public enum DebuffType
{
    Fire
}
