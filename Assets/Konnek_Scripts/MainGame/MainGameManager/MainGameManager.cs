using Unity.Netcode;
using UnityEngine;

public partial class MainGameManager : NetworkSingleton<MainGameManager>
{
    [Header("Game Context")]
    public MainGameContext MainGameContext;
    public CommandQueue CommandQueue;
    MainGameSetting mainGameSetting;
    public NetworkVariable<ulong> CurrentClientTurn = new();

    public bool IsTestOnePlayer;

    protected override void InitAfterAwake()
    {
    }

    private void Start()
    {
        if (IsTestOnePlayer)
            NetworkManager.StartHost();
        StartGame();

    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        mainGameSetting = Resources.Load<MainGameSetting>("Game Setting/Main Game Setting/SO/Main Game Setting");
        MainGameContext = new(PlayerManager.Instance.PlayerData_1, PlayerManager.Instance.PlayerData_2, mainGameSetting);
        CommandQueue = new();

        OnGameStart += (MainGameContext context) =>
        {
            KonnekManager.OnPlayPieceFailed += (ctx) => Debug.Log("Cant play this column"); ;
            MainGameContext.SetCurrentGameState(MainGameState.Player_1_Start_Turn);
        };

        OnStartTurn_Player_1 += (MainGameContext context) =>
        {
            SetCurrentPlayerContextClientRpc(1);
            MainGameContext.GetCurrentPlayerContext().IsPlayerTurn = true;
            UpdateCurrentPlayerClientRpc(context.PlayerContextByPlayerOrder[1].PlayerData.PlayerName.ToString());
            CurrentClientTurn.Value = context.PlayerContextByPlayerOrder[1].GetClientId();
            KonnekManager.Instance.SetPlayPieceButton_ServerRpc(true, context.PlayerContextByPlayerOrder[1].GetClientId());
            KonnekManager.Instance.SetEndTurnButton_ServerRpc(true, context.PlayerContextByPlayerOrder[1].GetClientId());
        };

        OnStartTurn_Player_2 += (MainGameContext context) =>
        {
            SetCurrentPlayerContextClientRpc(2);
            MainGameContext.GetCurrentPlayerContext().IsPlayerTurn = true;
            UpdateCurrentPlayerClientRpc(context.PlayerContextByPlayerOrder[2].PlayerData.PlayerName.ToString());
            CurrentClientTurn.Value = context.PlayerContextByPlayerOrder[2].GetClientId();
            KonnekManager.Instance.SetPlayPieceButton_ServerRpc(true, context.PlayerContextByPlayerOrder[2].GetClientId());
            KonnekManager.Instance.SetEndTurnButton_ServerRpc(true, context.PlayerContextByPlayerOrder[2].GetClientId());
        };

        OnDuringTurn_Player_1 += (MainGameContext context) =>
        {
            if (IsReadyToUpdateTimer())
            {
                UpdateDuringTurnTimerClientRpc((int)context.currentTurnDuration);
            }
            DuringTurnTimer(MainGameContext);
        };

        OnDuringTurn_Player_2 += (MainGameContext context) =>
        {
            if (IsReadyToUpdateTimer())
            {
                UpdateDuringTurnTimerClientRpc((int)context.currentTurnDuration);
            }
            DuringTurnTimer(MainGameContext);
        };

        OnEndTurn_Player_1 += (MainGameContext context) =>
        {
            MainGameContext.GetCurrentPlayerContext().IsPlayerTurn = false;
        };

        OnEndTurn_Player_2 += (MainGameContext context) =>
        {
            MainGameContext.GetCurrentPlayerContext().IsPlayerTurn = false;
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
        CommandQueue.AddCommand(command);
    }

    public void StartGame()
    {
        OnGameStart?.Invoke(MainGameContext);
    }

    [ClientRpc]
    private void SetCurrentPlayerContextClientRpc(int playerIndex)
    {
        MainGameContext.SetCurrentPlayerContext(playerIndex);
    }

}
