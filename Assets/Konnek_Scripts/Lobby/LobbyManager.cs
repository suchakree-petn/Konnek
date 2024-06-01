using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Konnek.KonnekLobby
{
    public partial class LobbyManager : NetworkSingleton<LobbyManager>
    {
        public const int MAX_PLAYER = 2;
        public const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
        public const string KEY_PLAYER_NAME = "PlayerName";
        public const string KEY_STAGE_ID = "StageId";
        private float heartbeatTimer;
        private float refreshTimer;
        private float checkInLobbyTimer;
        private float listLobbiesTimer;

        public event EventHandler OnCreateLobbyStarted;
        public event EventHandler OnCreateLobbyFailed;
        public event EventHandler OnJoinStarted;
        public event EventHandler OnQuickJoinFailed;
        public event EventHandler OnJoinFailed;
        public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
        public class LobbyEventArgs : EventArgs
        {
            public Lobby lobby;
        }
        public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
        public class OnLobbyListChangedEventArgs : EventArgs
        {
            public List<Lobby> lobbyList;
        }
        public Action OnPlayerReady;

        private Lobby joinedLobby;
        private List<string> joinedPlayerId = new();
        public NetworkList<PlayerLobby> joinedPlayerLobby = new();
        public Dictionary<ulong, PlayerLobby> JoinedPlayerLobby
        {
            get
            {
                List<PlayerLobby> tempJoinedList = new();
                foreach (PlayerLobby playerLobby in joinedPlayerLobby)
                {
                    tempJoinedList.Add(playerLobby);
                }
                Dictionary<ulong, PlayerLobby> temp = tempJoinedList.ToDictionary(keys => keys.ClientId, values => values);
                return temp;
            }
        }

        protected override void InitAfterAwake()
        {
            InitializeUnityAuthentication();
            joinedPlayerLobby = new NetworkList<PlayerLobby>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);



        }


        private void Update()
        {
            HandleLobbyRefresh();
            HandleLobbyHeartbeat();
            HandlePeriodicListLobbies();
            HandleCheckIsInLobby();
        }
        private void Start()
        {
            RefreshLobbyList();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            NetworkManager.OnClientConnectedCallback += HandleOnPlayerJoinedLobby;


        }
        private async void InitializeUnityAuthentication()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions initializationOptions = new();
                initializationOptions.SetProfile(UnityEngine.Random.Range(0, 100000).ToString());

                await UnityServices.InitializeAsync(initializationOptions);

                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        public async void CreateLobby(string lobbyName, bool isPrivate)
        {
            OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
            try
            {
                var options = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Player = new Player
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                    {
                        {KEY_PLAYER_NAME,new(PlayerDataObject.VisibilityOptions.Member,PlayerManager.Instance.LocalPlayerName)},
                    }
                    },
                    Data = new Dictionary<string, DataObject>
                    {
                        {KEY_STAGE_ID,new DataObject(DataObject.VisibilityOptions.Public,"Test Stage")},
                    }

                };
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYER, options);

                joinedLobby = lobby;

                OnJoinStarted?.Invoke(this, EventArgs.Empty);
                Debug.Log("PlayerCount: " + joinedLobby.Players.Count);

                Debug.Log("Stage ID: " + joinedLobby.Data[KEY_STAGE_ID].Value);

                foreach (Player player in joinedLobby.Players)
                {
                    Debug.Log("Player name: " + player.Data[KEY_PLAYER_NAME].Value);
                }
                Loader.LoadingScene();

                Allocation allocation = await AllocateRelay();
                string relayJoinCode = await GetRelayJoinCode(allocation);

                await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions()
                {
                    Data = new Dictionary<string, DataObject>{
                    {KEY_RELAY_JOIN_CODE,new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
                });
                NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
                Debug.Log("Create lobby code: " + joinedLobby.LobbyCode);
                KonnekMultiplayerManager.Instance.StartHost();

                NetworkManager.SceneManager.OnLoadComplete += LobbyManager_OnNetworkManagerOnLoadCompleted;

                Loader.LoadNetwork(Loader.Scene.LobbyScene);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning(e);
            }

        }

        private void LobbyManager_OnNetworkManagerOnLoadCompleted(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (Loader.TargetScene != Loader.Scene.LobbyScene)
            {
                Debug.LogWarning("Not in Lobby scene");
                return;
            }
            Debug.Log("Lobby scene");

            NetworkManager.SceneManager.OnLoadComplete -= LobbyManager_OnNetworkManagerOnLoadCompleted;

        }

        // private void OnNetworkManagerOnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        // {
        //     PlayerManager.Instance.OnClientConnect += LobbyManager_UI.Instance.HandleOnPlayerJoinedLobby;

        //     foreach (ulong clientId in clientsCompleted)
        //     {
        //         Debug.Log($"Client {clientId} connected");
        //         PlayerManager.Instance.OnClientConnect?.Invoke(clientId);
        //     }

        // }
        public async void DeleteLobby()
        {
            if (joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                    joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }
        public Lobby GetJoinedLobby()
        {
            return joinedLobby;
        }
        private async void HandleLobbyHeartbeat()
        {
            if (IsLobbyHost())
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f)
                {
                    float heartbeatTimerMax = 15f;
                    heartbeatTimer = heartbeatTimerMax;


                    Debug.Log("Heartbeat");
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
            }
        }
        private void HandleCheckIsInLobby()
        {
            if (SceneManager.GetActiveScene().name != "Thanva_InLobby") return;

            checkInLobbyTimer -= Time.deltaTime;
            if (checkInLobbyTimer < 0f)
            {
                float refreshTimerMax = 1.1f;
                checkInLobbyTimer = refreshTimerMax;
                if (!IsPlayerInLobby())
                {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    joinedLobby = null;
                }
            }
        }
        private async void HandleLobbyRefresh()
        {
            if (joinedLobby != null)
            {
                refreshTimer -= Time.deltaTime;
                if (refreshTimer < 0f)
                {
                    float refreshTimerMax = 1.1f;
                    refreshTimer = refreshTimerMax;
                    Lobby lobby = null;
                    try
                    {
                        lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                    }
                    catch
                    {
                        Debug.Log("Lobby not found");
                    }
                    joinedLobby = lobby;
                    RefreshLobbyList();
                }
            }
        }
        private bool IsPlayerInLobby()
        {
            if (joinedLobby != null && joinedLobby.Players != null)
            {
                foreach (Player player in joinedLobby.Players)
                {
                    if (player.Id == AuthenticationService.Instance.PlayerId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsLobbyHost()
        {
            return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }
        public async Task<JoinAllocation> JoinRelay(string joinCode)
        {
            try
            {
                return await RelayService.Instance.JoinAllocationAsync(joinCode);
            }
            catch (RelayServiceException e)
            {
                Debug.LogWarning(e);
                return default;
            }
        }
        public async void QuickJoin()
        {
            try
            {
                joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync(new()
                {
                    Player = new Player
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                    {
                        {KEY_PLAYER_NAME,new(PlayerDataObject.VisibilityOptions.Member,PlayerManager.Instance.LocalPlayerName)}
                    }
                    }
                });
                string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
                Debug.Log("Lobby Id: " + joinedLobby.Data[KEY_STAGE_ID].Value);
                Debug.Log("QJ code: " + relayJoinCode);
                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                KonnekMultiplayerManager.Instance.StartClient();

            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning(e);
            }
        }
        public async void JoinWithId(string lobbyId)
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            try
            {
                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

                string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                KonnekMultiplayerManager.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnJoinFailed?.Invoke(this, EventArgs.Empty);
            }
        }
        public async void JoinByCode(string lobbyCode)
        {
            try
            {
                joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new()
                {
                    Player = new Player
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                    {
                        {KEY_PLAYER_NAME,new(PlayerDataObject.VisibilityOptions.Member,PlayerManager.Instance.LocalPlayerName)}
                    }
                    }
                });
                Debug.Log("Join code: " + lobbyCode);

                string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                KonnekMultiplayerManager.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning(e);
            }
        }

        private async Task<Allocation> AllocateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYER - 1);
                return allocation;
            }
            catch (RelayServiceException e)
            {
                Debug.LogWarning(e);
                return default;
            }

        }
        private async Task<string> GetRelayJoinCode(Allocation allocation)
        {
            try
            {
                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return relayJoinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogWarning(e);
                return default;
            }

        }
        public async void LeaveLobby()
        {
            if (joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                    joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }
        public async void KickPlayer(string playerId)
        {
            if (IsLobbyHost())
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        public Lobby GetLobby()
        {
            if (joinedLobby == null) Debug.LogWarning("Joined Lobby is null on client: " + NetworkManager.LocalClientId);
            return joinedLobby;
        }
        public async void RefreshLobbyList()
        {
            try
            {
                QueryLobbiesOptions options = new()
                {
                    Count = 25,

                    // Filter for open lobbies only
                    Filters = new List<QueryFilter> {
                new(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            },

                    // Order by newest lobbies first
                    Order = new List<QueryOrder> {
                new(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            }
                };

                QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        private async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter> {
                  new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
             }
                };
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
                {
                    lobbyList = queryResponse.Results
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        private void HandlePeriodicListLobbies()
        {
            if (joinedLobby == null &&
                UnityServices.State == ServicesInitializationState.Initialized &&
                AuthenticationService.Instance.IsSignedIn &&
                SceneManager.GetActiveScene().name == "Thanva_MainMenu_UserDataPersistence")
            {

                listLobbiesTimer -= Time.deltaTime;
                if (listLobbiesTimer <= 0f)
                {
                    float listLobbiesTimerMax = 3f;
                    listLobbiesTimer = listLobbiesTimerMax;
                    ListLobbies();
                }
            }
        }
        public void HandleOnPlayerJoinedLobby(ulong clientId)
        {
            Debug.Log("Joined client: " + clientId);
            Lobby lobby = GetLobby();
            foreach (Player player in lobby.Players)
            {
                if (joinedPlayerId.Contains(player.Id))
                {
                    Debug.LogWarning("Already contain");
                    continue;
                }

                joinedPlayerLobby.Add(
                    new PlayerLobby()
                    {
                        ClientId = clientId,
                        PlayerName = player.Data[KEY_PLAYER_NAME].Value,
                        IsReady = false
                    });
                joinedPlayerId.Add(player.Id);
            }
        }
        public string GetKEY_PLAYER_NAME()
        {
            return KEY_PLAYER_NAME;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerReady_ServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;
            for (int i = 0; i < joinedPlayerLobby.Count; i++)
            {
                if (joinedPlayerLobby[i].ClientId == clientId)
                {
                    PlayerLobby playerLobby = joinedPlayerLobby[i];
                    playerLobby.IsReady = !playerLobby.IsReady;
                    joinedPlayerLobby[i] = playerLobby;
                    OnPlayerReady?.Invoke();
                    break;
                }
            }

        }
        // private void OnJoinedPlayerLobbyChanged(NetworkListEvent<PlayerLobby> changeEvent)
        // {
        //     if (changeEvent.Type == NetworkListEvent<PlayerLobby>.EventType.Add)
        //     {
        //         HandleOnPlayerJoinedLobby(changeEvent.Value.ClientId);
        //     }
        // }
    }
    public struct PlayerLobby : INetworkSerializable, IEquatable<PlayerLobby>
    {
        public ulong ClientId;
        public FixedString32Bytes PlayerName;
        public bool IsReady;

        public bool Equals(PlayerLobby other)
        {
            return other.ClientId == ClientId
            && other.PlayerName == PlayerName
            && other.IsReady == IsReady;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref IsReady);
        }
    }
}


