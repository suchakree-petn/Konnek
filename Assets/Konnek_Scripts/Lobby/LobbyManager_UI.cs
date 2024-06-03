using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Konnek.KonnekLobby
{
    public class LobbyManager_UI : NetworkSingleton<LobbyManager_UI>
    {

        [Header("UI")]
        [SerializeField] private UI_Config UI_Config;
        [SerializeField] private Transform parent;
        [SerializeField] private Transform playerLobby_prf;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI lobbyCode;


        protected override void InitAfterAwake()
        {

            readyButton.onClick.AddListener(() => SetPlayerReady(NetworkManager.LocalClientId));
            LobbyManager.Instance.OnPlayerLobbyListChanged += UpdatePlayerJoined;
        }

        private void Start()
        {
            SetLobbyCodeUI(LobbyManager.Instance.GetLobby().LobbyCode);
        }

        public override void OnNetworkSpawn()
        {
            UpdatePlayerJoined();
            if (!IsServer) return;
            NetworkManager.OnClientConnectedCallback += LobbyManager_UI_OnClientConnected_ClientRpc;

        }


        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            NetworkManager.OnClientConnectedCallback -= LobbyManager_UI_OnClientConnected_ClientRpc;

        }



        [ServerRpc(RequireOwnership = false)]
        public void UpdatePlayerJoined_ServerRpc(ulong clientId)
        {
            UpdatePlayerJoined_ClientRpc();

            if (IsAllPlayerReady())
            {
                LoadingSceneTransition_ClientRpc();
                Loader.LoadNetwork(Loader.Scene.GameScene);
            }
        }

        [ClientRpc]
        private void LoadingSceneTransition_ClientRpc()
        {
            Loader.LoadingScene();
        }

        private bool IsAllPlayerReady()
        {
            int playerReadyCount = 0;
            NetworkList<PlayerLobby> joinedPlayerLobby = LobbyManager.Instance.joinedPlayerLobby;
            foreach (PlayerLobby playerLobby in joinedPlayerLobby)
            {
                if (playerLobby.IsReady)
                {
                    playerReadyCount++;
                }
            }

            if (playerReadyCount == LobbyManager.MAX_PLAYER)
            {
                return true;
            }
            return false;
        }

        [ClientRpc]
        private void UpdatePlayerJoined_ClientRpc()
        {
            UpdatePlayerJoined();
        }
        public void UpdatePlayerJoined()
        {
            Debug.Log("Update lobby UI");
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
            foreach (PlayerLobby player in LobbyManager.Instance.joinedPlayerLobby)
            {
                Debug.Log($"{player.PlayerName} : {player.IsReady}");

                Transform playerLobby = Instantiate(playerLobby_prf, parent);
                TextMeshProUGUI playerName = playerLobby.GetComponent<TextMeshProUGUI>();
                playerName.text = player.PlayerName.ToString();
                Image readyCheckMark = playerLobby.GetChild(1).GetComponent<Image>();
                TextMeshProUGUI readyText = playerLobby.GetChild(2).GetComponent<TextMeshProUGUI>();
                if (player.IsReady)
                {
                    readyCheckMark.color = UI_Config.ReadyColor;
                    readyText.text = UI_Config.ReadyText;
                }
                else
                {
                    readyCheckMark.color = UI_Config.NotReadyColor;
                    readyText.text = UI_Config.NotReadyText;
                }
            }

        }

        [ClientRpc]
        private void LobbyManager_UI_OnClientConnected_ClientRpc(ulong clientId)
        {
            UpdatePlayerJoined();
        }

        private void SetLobbyCodeUI(string code)
        {
            lobbyCode.text = "Code: " + code;
        }
        public void SetPlayerReady(ulong clientId)
        {
            NetworkList<PlayerLobby> joinedPlayerLobby = LobbyManager.Instance.joinedPlayerLobby;
            for (int i = 0; i < joinedPlayerLobby.Count; i++)
            {
                if (joinedPlayerLobby[i].ClientId == clientId)
                {
                    Debug.Log($"client {clientId} write on client {joinedPlayerLobby[i].ClientId}");
                    PlayerLobby playerLobby = joinedPlayerLobby[i];
                    playerLobby.IsReady = !playerLobby.IsReady;
                    LobbyManager.Instance.SetPlayerLobby_ServerRpc(i, playerLobby);
                    break;
                }
            }
            UpdatePlayerJoined_ServerRpc(clientId);
        }

    }
    [Serializable]
    public class UI_Config
    {
        public Color ReadyColor;
        public Color NotReadyColor;
        public string ReadyText;
        public string NotReadyText;
    }
}


