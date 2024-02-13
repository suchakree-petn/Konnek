using Konnek.Util;
using Konnek.Lobby;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Konnek.MainMenu
{
    public partial class MainMenuManager : Singleton<MainMenuManager>
    {
        [Header("Lobby")]
        [SerializeField] private Button startHost;
        [SerializeField] private Button startClient;
        [SerializeField] private TMP_InputField userName, lobbyCode;


        protected override void InitAfterAwake()
        {
            startHost.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(userName != null ? userName.text : null))
                {
                    return;
                }

                NetworkManager.Singleton.NetworkConfig.ConnectionData = GetPayLoad();
                NetworkManager.Singleton.ConnectionApprovalCallback += OnClientConnectApprove;
                NetworkManager.Singleton.StartHost();
                NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;

                NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            });

            startClient.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(userName != null ? userName.text : null))
                {
                    return;
                }
                NetworkManager.Singleton.NetworkConfig.ConnectionData = GetPayLoad();
                NetworkManager.Singleton.StartClient();

            });
        }

        private void SceneManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            Debug.Log($"Test load complete {clientId} connected to {sceneName}; Mode: {loadSceneMode}");
            string[] payload = KonnekUtil.GetStringArrayFromByteArray(GetPayLoad(), ' ');
            LobbyManager.Instance.InitPlayerLobbyServerRpc(ulong.Parse(payload[0]),clientId);
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManager_OnLoadComplete;
        }


    }
}

