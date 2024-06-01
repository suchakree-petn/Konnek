using Konnek.Util;
using Konnek.KonnekLobby;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Konnek.MainMenu
{
    public partial class MainMenuManager : Singleton<MainMenuManager>
    {
        [Header("Play UI")]
        [SerializeField] private Button startHost;
        [SerializeField] private Button startClient;
        [SerializeField] private TMP_InputField lobbyName;
        [SerializeField] private Toggle isPrivateToggle;
        public TMP_InputField NameInputField;


        protected override void InitAfterAwake()
        {
            KonnekMultiplayerManager.Instance.SetPlayerName(NameInputField.text);

            NameInputField.onValueChanged.AddListener((string newText) =>
            {
                KonnekMultiplayerManager.Instance.SetPlayerName(newText);
            });
            startHost.onClick.AddListener(() =>
            {
                LobbyManager.Instance.CreateLobby(lobbyName.text, isPrivateToggle.isOn);
            });

            startClient.onClick.AddListener(() =>
            {
                LobbyManager.Instance.QuickJoin();
            });
        }


    }
}

