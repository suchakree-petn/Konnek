using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Konnek.Lobby
{
    public partial class LobbyManager
    {
        [Header("UI")]
        [SerializeField] private UI_Config UI_Config;
        [SerializeField] private Transform parent;
        [SerializeField] private Transform playerLobby_prf;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI lobbyCode;

        [ClientRpc]
        private void PlayerJoinedUI_ClientRpc(int clientCount)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < clientCount; i++)
            {
                PlayerLobby player = _host.Value;
                if (i != 0)
                {
                    player = _client.Value;
                }
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
        private void UpdateHostReadyUI_ClientRpc(PlayerLobby newValue)
        {
            Debug.Log("host change");
            Transform PlayerLobbyObj = parent.GetChild(0);
            Image readyCheckMark = PlayerLobbyObj.GetChild(1).GetComponent<Image>();
            TextMeshProUGUI readyText = PlayerLobbyObj.GetChild(2).GetComponent<TextMeshProUGUI>();
            if (newValue.IsReady)
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
        [ClientRpc]
        private void UpdateClientReadyUI_ClientRpc(PlayerLobby newValue)
        {
            Debug.Log("client change");

            Transform PlayerLobbyObj = parent.GetChild(1);
            Image readyCheckMark = PlayerLobbyObj.GetChild(1).GetComponent<Image>();
            TextMeshProUGUI readyText = PlayerLobbyObj.GetChild(2).GetComponent<TextMeshProUGUI>();
            if (newValue.IsReady)
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

        [ClientRpc]
        private void InitLobbyCodeUI_ClientRpc(string code)
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


