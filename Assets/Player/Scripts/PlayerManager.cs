using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkSingleton<PlayerManager>
{
    public PlayerData playerData_1;
    public PlayerData playerData_2;
    public PlayerData OwnerPlayerData;

    public Action OnFinishInitAllPlayerData;
    protected override void InitAfterAwake()
    {
    }

    private void Start()
    {
        if (playerData_1 != null || playerData_2 != null)
        {
            InitOwnerPlayerData();
            OnFinishInitAllPlayerData?.Invoke();
        }
    }

    [ClientRpc]
    public void InitPlayerData_ClientRpc(PlayerData playerData)
    {
        _ = ScriptableObject.CreateInstance<PlayerData>();
        PlayerData data = playerData;
        if (playerData_1 == null)
        {
            playerData_1 = data;
        }
        else
        {
            playerData_2 = data;
        }
    }
    private void InitOwnerPlayerData()
    {
        OwnerPlayerData = IsServer ? playerData_1 : playerData_2;
    }
    public static ulong GetOwnerPlayerId()
    {
        return Instance.OwnerPlayerData.PlayerId;
    }
    public static ulong GetPlayerId(int playerOrderIndex)
    {
        return playerOrderIndex == 1 ? Instance.playerData_1.PlayerId : Instance.playerData_2.PlayerId;
    }

}
