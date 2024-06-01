using Unity.Netcode;
using UnityEngine;

public partial class MainGameManager : NetworkSingleton<MainGameManager>
{
    [Header("Game Context")]
    public MainGameContext mainGameContext;
    public CommandQueue commandQueue;
    MainGameSetting mainGameSetting;

    protected override void InitAfterAwake()
    {
    }


    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        mainGameSetting = Resources.Load<MainGameSetting>("Game Setting/Main Game Setting/SO/Main Game Setting");
        mainGameContext = new(PlayerManager.Instance.playerData_1, PlayerManager.Instance.playerData_2, mainGameSetting);
        commandQueue = new();

        OnGameStart += (ref MainGameContext context) =>
        {
            KonnekManager.OnPlayPieceFailed += (ctx) => Debug.Log("Cant play this column"); ;
            mainGameContext.currentState = MainGameState.Player_1_Start_Turn;
        };

        OnStartTurn_Player_1 += (ref MainGameContext context) =>
        {
            SetCurrentPlayerContextClientRpc(1);
            context.GetCurrentPlayerContext().isPlayerTurn = true;
            UpdateCurrentPlayerClientRpc("Name");
        };

        OnStartTurn_Player_2 += (ref MainGameContext context) =>
        {
            SetCurrentPlayerContextClientRpc(2);
            context.GetCurrentPlayerContext().isPlayerTurn = true;
            UpdateCurrentPlayerClientRpc("Name");
        };

        OnDuringTurn_Player_1 += (ref MainGameContext context) =>
        {
            DuringTurnTimer(mainGameContext);
            UpdateDuringTurnTimerClientRpc((int)context.currentTurnDuration);
        };

        OnDuringTurn_Player_2 += (ref MainGameContext context) =>
        {
            DuringTurnTimer(mainGameContext);
            UpdateDuringTurnTimerClientRpc((int)context.currentTurnDuration);
        };

        OnEndTurn_Player_1 += (ref MainGameContext context) =>
        {
            context.GetCurrentPlayerContext().isPlayerTurn = false;
        };

        OnEndTurn_Player_2 += (ref MainGameContext context) =>
        {
            context.GetCurrentPlayerContext().isPlayerTurn = false;
        };

    }

    private void Update()
    {
        if (!IsServer) return;
        GameStateRunner();
    }

    public void AddCommand(Command command, Command.OnCompleteCallback onCompleteCallback = null)
    {
        if (onCompleteCallback != null)
        {
            command.OnComplete(onCompleteCallback);
        }
        else
        {
            Debug.Log("No On complete");
        }
        commandQueue.AddCommand(command);
    }

    public void StartGame()
    {
        OnGameStart?.Invoke(ref mainGameContext);
    }

    [ClientRpc]
    private void SetCurrentPlayerContextClientRpc(int playerIndex)
    {
        mainGameContext.SetCurrentPlayerContext(playerIndex);
    }

}
