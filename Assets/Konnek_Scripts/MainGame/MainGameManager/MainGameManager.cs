using Unity.Netcode;
using UnityEngine;

public partial class MainGameManager : NetworkSingleton<MainGameManager>
{
    [Header("Game Context")]
    public MainGameContext MainGameContext;
    public CommandQueue CommandQueue;
    public CommandQueue AnimationQueue;
    MainGameSetting mainGameSetting;
    private NetworkVariable<ulong> currentClientTurn = new();
    public ulong CurrentClientTurn => currentClientTurn.Value;
    [SerializeField] private NetworkVariable<MainGameState> mainGameState = new(MainGameState.Default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public bool IsTestOnePlayer;

    protected override void InitAfterAwake()
    {
    }

    private void Start()
    {
        StartGame();
    }
    public override void OnNetworkSpawn()
    {
        AnimationQueue = new();

        if (!IsServer) return;
        mainGameSetting = Resources.Load<MainGameSetting>("Game Setting/Main Game Setting/SO/Main Game Setting");
        MainGameContext = new(PlayerManager.Instance.PlayerData_1, PlayerManager.Instance.PlayerData_2, mainGameSetting);
        CommandQueue = new();

        KonnekUIManager konnekUIManager = KonnekUIManager.Instance;

        OnStartTurn_Player_1 += (MainGameContext context) =>
        {
            MainGameContext.SetCurrentPlayerContext(1);
            MainGameContext.GetCurrentPlayerContext().IsPlayerTurn = true;
            UpdateCurrentPlayerTextClientRpc(context.PlayerContextByPlayerOrder[1].PlayerData.PlayerName.ToString());
            currentClientTurn.Value = context.PlayerContextByPlayerOrder[1].GetClientId();

            konnekUIManager.StartPlayerTurnAnimation(context);
            konnekUIManager.SetPlayPieceButton_ServerRpc(true, context.PlayerContextByPlayerOrder[1].GetClientId());
            konnekUIManager.SetEndTurnButton_ServerRpc(true, context.PlayerContextByPlayerOrder[1].GetClientId());
        };

        OnStartTurn_Player_2 += (MainGameContext context) =>
        {
            MainGameContext.SetCurrentPlayerContext(2);
            MainGameContext.GetCurrentPlayerContext().IsPlayerTurn = true;
            UpdateCurrentPlayerTextClientRpc(context.PlayerContextByPlayerOrder[2].PlayerData.PlayerName.ToString());
            currentClientTurn.Value = context.PlayerContextByPlayerOrder[2].GetClientId();

            konnekUIManager.StartPlayerTurnAnimation(context);
            konnekUIManager.SetPlayPieceButton_ServerRpc(true, context.PlayerContextByPlayerOrder[2].GetClientId());
            konnekUIManager.SetEndTurnButton_ServerRpc(true, context.PlayerContextByPlayerOrder[2].GetClientId());
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
            // Debug.Log("No On complete");
        }
        CommandQueue.AddCommand(command);
    }
    public void AddAnimationCommand(Command command, Command.OnCompleteCallback onCompleteCallback = null)
    {
        if (onCompleteCallback != null)
        {
            command.OnComplete(onCompleteCallback);
        }
        else
        {
            // Debug.Log("No On complete");
        }
        AnimationQueue.AddCommand(command);
    }
    public void StartGame()
    {
        SetCurrentGameState(MainGameState.StartGame);
    }

    public void SetCurrentGameState(MainGameState state)
    {
        Debug.Log("Set game state to " + state.ToString());
        mainGameState.Value = state;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCurrentGameState_ServerRpc(MainGameState state)
    {
        SetCurrentGameState(state);
    }

    public MainGameState GetCurrentGameState()
    {
        return mainGameState.Value;
    }

}
