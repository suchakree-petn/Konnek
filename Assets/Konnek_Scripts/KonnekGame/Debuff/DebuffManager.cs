using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DebuffManager : NetworkSingleton<DebuffManager>
{
    public List<Debuff> Debuffs => DebuffIdDict.Values.ToList();
    public static Dictionary<uint, GameObject> DebuffGameObjectDict = new();
    public static Dictionary<uint, Debuff> DebuffIdDict = new();

    private static uint debuffInstanceId = 0;
    public static uint GetDebuffInstanceId()
    {
        uint currentId = debuffInstanceId;
        debuffInstanceId++;
        return currentId;
    }

    protected override void InitAfterAwake()
    {
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        MainGameManager mainGameManager = MainGameManager.Instance;
        KonnekManager konnekManager = KonnekManager.Instance;

        // debuffs trigger event
        mainGameManager.OnStartTurn_Player_1 += HandleDebuffProc;
        mainGameManager.OnStartTurn_Player_2 += HandleDebuffProc;
    }

    private void HandleDebuffProc(MainGameContext ctx)
    {
        Debug.Log("Proc count: " + Debuffs.Count);
        Debuff[] temp = new Debuff[Debuffs.Count];
        Debuffs.CopyTo(temp);
        foreach (Debuff debuff in temp)
        {
            debuff.Proc();
        }
    }

    public void AddDebuff(DebuffType debuffType, int damage, ulong userClientId, int duration)
    {
        OnAddDebuff_ClientRpc(debuffType, damage, userClientId, duration);
    }

    [ClientRpc]
    private void OnAddDebuff_ClientRpc(DebuffType debuffType, int damage, ulong userClientId, int duration)
    {
        Debuff debuff = GetDebuff(debuffType, damage, userClientId, duration);
        DebuffIdDict.Add(debuff.InstanceId, debuff);
        debuff.OnAddDebuff();
        Debug.Log("Debuff count: " + Debuffs.Count);
    }

    public void RemoveDebuff(uint instanceId)
    {
        OnRemoveDebuff_ClientRpc(instanceId);
    }

    [ClientRpc]
    private void OnRemoveDebuff_ClientRpc(uint instanceId)
    {
        Debuff debuff = DebuffIdDict[instanceId];
        DebuffIdDict.Remove(instanceId);
        debuff.OnRemoveDebuff();
    }

    private Debuff GetDebuff(DebuffType debuffType, int damage, ulong userClientId, int duration)
    {
        switch (debuffType)
        {
            case DebuffType.Fire:
                return new FireDebuff(damage, userClientId, duration);

        }
        return null;
    }
}
