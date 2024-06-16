using UnityEngine;

public class FireDebuff : Debuff
{

    public FireDebuff(int damage, ulong userClientId, int duration) : base(damage, userClientId, duration)
    {
        Debug.Log("Init damage" + damage);

    }

    public override DebuffType Type { get => DebuffType.Fire; }

    public override void OnAddDebuff()
    {
        Command command = new Card_AddFirePieceAnimation(InstanceId);
        MainGameManager.Instance.AddAnimationCommand(command);
    }

    public override void OnRemoveDebuff()
    {
        Command command = new Card_RemoveFirePieceAnimation(InstanceId);
        MainGameManager.Instance.AddAnimationCommand(command);
    }

    public override void Proc()
    {
        if (MainGameManager.Instance.CurrentClientTurn == OpponentClientId && Duration > 0)
        {
            KonnekManager.Instance.HpDecrease_ServerRpc(OpponentClientId, Damage);
            Duration--;
            if (Duration <= 0)
            {
                DebuffManager.Instance.RemoveDebuff(InstanceId);
            }
        }
    }


}
