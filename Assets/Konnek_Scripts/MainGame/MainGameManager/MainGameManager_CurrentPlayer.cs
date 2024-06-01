using TMPro;
using Unity.Netcode;
using UnityEngine;

public partial class MainGameManager
{
    [Header("Current player UI reference")]
    [SerializeField] private TextMeshProUGUI currentPlayer;

    [ClientRpc]
    public void UpdateCurrentPlayerClientRpc(string currentPlayerName)
    {
        currentPlayer.text = "Current Player:" + currentPlayerName;
    }
}