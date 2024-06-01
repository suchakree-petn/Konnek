using TMPro;
using Unity.Netcode;
using UnityEngine;

public partial class MainGameManager
{
    [Header("Turn-timer reference")]
    [SerializeField] private TextMeshProUGUI duringTurnTimer;

    [ClientRpc]
    public void UpdateDuringTurnTimerClientRpc(int currentTurnDuration)
    {
        duringTurnTimer.text = currentTurnDuration.ToString();
    }
    public void DuringTurnTimer(MainGameContext context)
    {
        if (context.currentTurnDuration > 0)
        {
            context.currentTurnDuration -= Time.deltaTime;
        }
        else
        {
            Server_EndTurn(ref context);
        }
    }
}
