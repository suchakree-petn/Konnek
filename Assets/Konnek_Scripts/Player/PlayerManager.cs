using System;
using Konnek.KonnekLobby;
using Konnek.MainMenu;
using Unity.Netcode;
using UnityEngine;

public partial class PlayerManager : NetworkSingleton<PlayerManager>
{
    public PlayerData playerData_1;
    public PlayerData playerData_2;
    public PlayerData LocalPlayerData => new() { PlayerName = MainMenuManager.Instance.NameInputField.text };
    public string LocalPlayerName => MainMenuManager.Instance.NameInputField.text;

    public Action<ulong> OnClientConnect;
    protected override void InitAfterAwake()
    {

    }

    private void Start()
    {
        // InitOwnerPlayerData();
        // OnFinishInitAllPlayerData?.Invoke();
    }
    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

    }

    public override void OnNetworkDespawn()
    {
        if(!IsServer) return;

    }
    // [ClientRpc]
    // public void InitPlayerData_ClientRpc(ulong playerId)
    // {
    //     PlayerData data = PlayerData.Cache[playerId];
    //     if (playerData_1 == null)
    //     {
    //         playerData_1 = data;
    //     }
    //     else
    //     {
    //         playerData_2 = data;
    //     }
    // }
    // private void InitOwnerPlayerData()
    // {
    //     LocalPlayerData = IsServer ? playerData_1 : playerData_2;
    // }


}
