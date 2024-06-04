using TMPro;
using Unity.Netcode;
using UnityEngine;

public partial class MainGameManager
{

    [Header("Turn-timer")]
    [SerializeField] private float timerInterval;
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
            KonnekManager.Instance.EndTurn_ServerRpc(context.GetCurrentPlayerContext().GetClientId());
        }
    }

    internal bool IsReadyToUpdateTimer()
    {
        bool isReady = false;
        if (timerInterval < 1)
        {
            timerInterval += Time.deltaTime;
        }
        else
        {
            timerInterval = 0;
            isReady = true;
        }
        return isReady;
    }
}
