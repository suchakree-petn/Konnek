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
        MainGameState mainGameState = GetCurrentGameState();
        if (context.currentTurnDuration > 0
        && (mainGameState == MainGameState.Player_1_During_Turn || mainGameState == MainGameState.Player_2_During_Turn))
        {
            if (AnimationQueue.CurrentExecutingCommand != StartTurnAnimation.START_TURN_ANIMATION_NAME)
            {
                context.currentTurnDuration -= Time.deltaTime;
            }
        }
        else
        {
            Debug.Log("Time out");
            KonnekManager.Instance.EndTurn(CurrentClientTurn);
            KonnekUIManager.Instance.SetEndTurnButton_ClientRpc(false, CurrentClientTurn);
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
