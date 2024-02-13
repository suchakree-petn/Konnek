using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Konnek.Lobby
{
    public partial class LobbyManager : NetworkSingleton<LobbyManager>
    {
        public string LobbyCode;
        [SerializeField] private NetworkVariable<PlayerLobby> _host = new(new());
        [SerializeField] private NetworkVariable<PlayerLobby> _client = new(new());
        public PlayerLobby Host => _host.Value;
        public PlayerLobby Client => _client.Value;
        // {
        //     get
        //     {
        //         return _client.Value;
        //     }
        //     set
        //     {
        //         _client.Value = value;
        //     }
        // }
        public Action<int> OnPlayerJoinedLobby;
        public Action<PlayerLobby> OnHostReadyChanged;
        public Action<PlayerLobby> OnClientReadyChanged;
        protected override void InitAfterAwake()
        {
            readyButton.onClick.AddListener(() =>
            {
                ReadyServerRpc();
            });

        }
        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            NetworkManager.OnClientConnectedCallback += OnPlayerJoinedLobby_ServerRpc;
            OnPlayerJoinedLobby += (clientId) =>
            {
                PlayerJoinedUI_ClientRpc(NetworkManager.ConnectedClients.Count);
                InitLobbyCodeUI_ClientRpc(LobbyCode);
            };
            OnHostReadyChanged += UpdateHostReadyUI_ClientRpc;
            OnClientReadyChanged += UpdateClientReadyUI_ClientRpc;
            OnHostReadyChanged += CheckReadyToEnterGame;
            OnClientReadyChanged += CheckReadyToEnterGame;
        }

        private void CheckReadyToEnterGame(PlayerLobby lobby)
        {
            Debug.Log("Check enter game");
            if (Host.IsReady && Client.IsReady)
            {
                Debug.Log("Everyone was ready. Entering the game");
                NetworkManager.SceneManager.LoadScene("KonnekGame", LoadSceneMode.Single);
            }
        }

        [ServerRpc]
        private void OnPlayerJoinedLobby_ServerRpc(ulong clientId)
        {
            OnPlayerJoinedLobby?.Invoke((int)clientId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void InitPlayerLobbyServerRpc(ulong playerId, ulong clientId)
        {
            PlayerData playerData = PlayerData.Cache[playerId];
            PlayerManager.Instance.InitPlayerData_ClientRpc(playerData);
            if (clientId == NetworkManager.ServerClientId)
            {
                byte[] payload = NetworkManager.NetworkConfig.ConnectionData;
                string[] payloadString = System.Text.Encoding.UTF8.GetString(payload).Split(" ");
                LobbyCode = payloadString[1];
                Debug.Log("Init lobby code");
                _host.Value = new(playerData.PlayerName, playerData.PlayerId, clientId);
            }
            else
            {
                _client.Value = new(playerData.PlayerName, playerData.PlayerId, clientId);
            }

            Debug.Log($"Init {playerId}");
            OnPlayerJoinedLobby_ServerRpc(clientId);

        }

        [ServerRpc(RequireOwnership = false)]
        public void ReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (serverRpcParams.Receive.SenderClientId == NetworkManager.ServerClientId)
            {
                Debug.Log("Host " + Host.IsReady);
                _host.Value.IsReady = !Host.IsReady;
                OnHostReadyChanged?.Invoke(Host);
            }
            else
            {
                Debug.Log("Client " + Client.IsReady);
                _client.Value.IsReady = !Client.IsReady;
                OnClientReadyChanged?.Invoke(Client);
            }
        }
    }

    [Serializable]
    public class PlayerLobby : INetworkSerializable
    {
        public FixedString128Bytes PlayerName = default;
        public ulong PlayerId = default;
        public ulong ClientId = default;
        public bool IsReady = false;

        public PlayerLobby(FixedString128Bytes playerName, ulong playerId, ulong clientId, bool isReady = false)
        {
            PlayerName = playerName;
            PlayerId = playerId;
            ClientId = clientId;
            IsReady = isReady;
        }
        public PlayerLobby()
        {
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref PlayerId);
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref IsReady);
        }
    }
}

