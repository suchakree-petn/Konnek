using Konnek.Lobby;
using Unity.Netcode;
using UnityEngine;

namespace Konnek.MainMenu
{
    public partial class MainMenuManager
    {
        private byte[] GetPayLoad()
        {
            // let username = playerId
            string userName = this.userName.text;
            string lobbyCode = this.lobbyCode.text;
            string payLoad = default;
            if (ulong.TryParse(userName.Trim(), out ulong playerId))
            {
                payLoad = string.Concat(playerId, " ", lobbyCode.Trim());

            }
            return System.Text.Encoding.UTF8.GetBytes(payLoad);
        }
        private void OnClientConnectApprove(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {

            string payLoad;
            if (request.Payload.Length > 0)
            {
                payLoad = System.Text.Encoding.UTF8.GetString(request.Payload);
            }
            else
            {
                response.Approved = false;
                response.Reason = "Payload not found";
                return;
            }

            string[] payLoadString = payLoad.Split(" ");
            ulong playerId = ulong.Parse(payLoadString[0]);
            if (request.ClientNetworkId == NetworkManager.ServerClientId)
            {
                response.Approved = true;
                return;
            }

            if (payLoadString[1] == LobbyManager.Instance.LobbyCode && playerId != LobbyManager.Instance.Host.PlayerId)
            {
                Debug.Log("Init Client");
                LobbyManager.Instance.InitPlayerLobbyServerRpc(playerId,request.ClientNetworkId);
                
                // LobbyManager.Instance.Client.PlayerName = payLoadString[0];
                // LobbyManager.Instance.Client.ClientId = (int)request.ClientNetworkId;
                response.Approved = true;
                return;
            }
            else
            {
                response.Approved = false;
                response.Reason = $"Fail connection\n{payLoadString[0]} : Disconnected";
            }

        }
    }
}

