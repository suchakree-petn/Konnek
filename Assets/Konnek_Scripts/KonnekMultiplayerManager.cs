using System;
using Konnek.KonnekLobby;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class KonnekMultiplayerManager : NetworkSingleton<KonnekMultiplayerManager>
{
    private const string KEY_PLAYER_NAME = "PlayerNameMultiplayer";

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;


    public NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;


    protected override void InitAfterAwake()
    {
        playerDataNetworkList = new NetworkList<PlayerData>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    public string GetPlayerName()
    {
        if (playerName == "") return "Undefied Player Name";
        return playerName;
    }
    public NetworkList<PlayerData> GetPlayerDataNetworkList()
    {
        return playerDataNetworkList;
    }
    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;

        PlayerPrefs.SetString(KEY_PLAYER_NAME, playerName);
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        // NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.StartHost();
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            // NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
            NetworkManager.OnClientConnectedCallback -= NetworkManager_Server_OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;
        }
        if (IsClient)
        {
            NetworkManager.OnClientConnectedCallback -= NetworkManager_Client_OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;
        }


    }
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        if(playerDataNetworkList == null) return;
        
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.ClientId == clientId)
            {
                // Disconnected!
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            ClientId = clientId,
        });
        SetPlayerNameServerRpc(new NetworkString(GetPlayerName()));
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    // private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    // {
    //     if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
    //     {
    //         connectionApprovalResponse.Approved = false;
    //         connectionApprovalResponse.Reason = "Game has already started";
    //         return;
    //     }

    //     if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
    //     {
    //         connectionApprovalResponse.Approved = false;
    //         connectionApprovalResponse.Reason = "Game is full";
    //         return;
    //     }

    //     connectionApprovalResponse.Approved = true;
    // }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }




    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(new NetworkString(GetPlayerName()));
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(NetworkString playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.PlayerName = playerName.Value;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.PlayerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }


    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].ClientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.ClientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    // public void KickPlayer(ulong clientId) {
    //     NetworkManager.Singleton.DisconnectClient(clientId);
    //     NetworkManager_Server_OnClientDisconnectCallback(clientId);
    // }

}
public struct NetworkString : INetworkSerializable
{
    public FixedString32Bytes Value;

    public NetworkString(FixedString32Bytes value)
    {
        Value = value;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Value);
    }
}
