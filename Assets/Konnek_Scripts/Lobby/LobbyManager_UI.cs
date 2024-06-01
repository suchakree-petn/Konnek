using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

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

            readyButton.onClick.AddListener(() => LobbyManager.Instance.SetPlayerReady_ServerRpc());
        }

        private void Start()
        {
            SetLobbyCodeUI(LobbyManager.Instance.GetLobby().LobbyCode);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            LobbyManager.Instance.OnPlayerReady += UpdatePlayerJoined_ClientRpc;
            NetworkManager.SceneManager.OnLoadComplete += LobbyManager_OnLoadComplete;

            UpdatePlayerJoined_ClientRpc();
            NetworkManager.OnClientConnectedCallback += LobbyManager_UI_OnClientConnected;
        }

        private void LobbyManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (Loader.TargetScene != Loader.Scene.LobbyScene)
            {
                NetworkManager.SceneManager.OnLoadComplete -= LobbyManager_OnLoadComplete;
                Destroy(gameObject);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            LobbyManager.Instance.OnPlayerReady -= UpdatePlayerJoined_ClientRpc;
            NetworkManager.OnClientConnectedCallback -= LobbyManager_UI_OnClientConnected;

        }

        [ClientRpc]
        public void UpdatePlayerJoined_ClientRpc()
        {

            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
            List<Player> joinedPlayer = LobbyManager.Instance.GetLobby().Players;
            foreach (PlayerLobby player in LobbyManager.Instance.joinedPlayerLobby)
            {
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

        private void LobbyManager_UI_OnClientConnected(ulong clientId){
            UpdatePlayerJoined_ClientRpc();
        }

        private void SetLobbyCodeUI(string code)
        {
            lobbyCode.text = "Code: " + code;
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


