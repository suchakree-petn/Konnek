using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class KonnekManager : NetworkSingleton<KonnekManager>
{
    public delegate void KonnekDelegate(MainGameContext context);
    public static KonnekDelegate OnPlayPieceSuccess;
    public static KonnekDelegate OnPlayPieceFailed;
    public static KonnekDelegate OnPlayCardSuccess;
    public static KonnekDelegate OnPlayCardFailed;
    public static KonnekDelegate OnInitPlayerFinish;
    public Action OnKonnekBoardListChanged;

    [Header("Konnek Board")]
    public NetworkList<Vector3> KonnekBoard;
    public Dictionary<int, int> ColumnAmount = new();
    [SerializeField] private List<Vector3> localKonnekBoard = new();
    public List<Vector3> PlayedPositions
    {
        get
        {
            List<Vector3> temp = new();
            foreach (var item in KonnekBoard)
            {
                temp.Add(item);
            }
            return temp;
        }
    }
    public NetworkVariable<int> selectedColumn = new(4);
    public int SelectedColumn
    {
        get
        {
            return selectedColumn.Value;
        }
        set
        {
            if (value >= 1 && value <= 7)
            {
                Debug.Log($"Selected column is set to {value}");
                selectedColumn.Value = value;
            }
            else
            {
                Debug.LogWarning("Selected column cant be less than 1 or more than 7");
            }
        }
    }
    public KonnekBuilder konnekBuilder;

    protected override void InitAfterAwake()
    {
        playerActions = new();
        playerActions.InGame.Enable();
        KonnekBoard = new NetworkList<Vector3>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        KonnekBoard.OnListChanged += KonnekManager_OnKonnekBoardListChanged;
        OnKonnekBoardListChanged += UpdateLocalKonnekBoard;


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HpDecrease_ServerRpc(NetworkManager.LocalClientId, 5);
        }
    }

    public override void OnNetworkSpawn()
    {
        playerActions.InGame.SelectLeftColumn.performed += (ctx) => RequestMoveColumnServerRpc(SelectedColumn - 1);
        playerActions.InGame.SelectRightColumn.performed += (ctx) => RequestMoveColumnServerRpc(SelectedColumn + 1);

        if (!IsServer) return;
        CreateNewBoard();
        OnPlayPieceSuccess += CheckKonnek;

        MainGameManager mainGameManager = MainGameManager.Instance;
        DeckManager deckManager = DeckManager.Instance;
        KonnekUIManager konnekUIManager = KonnekUIManager.Instance;

        mainGameManager.OnStartGame += (MainGameContext context) =>
        {
            OnPlayPieceFailed += (ctx) => Debug.Log("Cant play this column");

            // Draw starting card
            OnInitPlayerFinish += (ctx) => mainGameManager.SetCurrentGameState(MainGameState.Player_1_Start_Turn);

            deckManager.DrawCardFromTopDeckServerRpc(
                context.PlayerContextByPlayerOrder[1].GetClientId()
                , MainGameContext.START_CARD_AMOUNT);

            deckManager.DrawCardFromTopDeckServerRpc(
                context.PlayerContextByPlayerOrder[2].GetClientId()
                , MainGameContext.START_CARD_AMOUNT);

            // Ready to start player 1 turn
            Debug.Log("Animation queue count: " + mainGameManager.AnimationQueue.commandsQueue.Count);
            mainGameManager.AnimationQueue.GetLastCommand().OnComplete(() => OnInitPlayerFinish?.Invoke(context));
        };
        mainGameManager.OnStartTurn_Player_1 += (MainGameContext context) =>
        {
            PlayerContext playerContext = context.PlayerContextByPlayerOrder[1];

            // Start turn animation
            konnekUIManager.StartPlayerTurnAnimation_ClientRpc();

            // Draw card when start turn
            deckManager.DrawCardFromTopDeckServerRpc(
                playerContext.GetClientId()
                , playerContext.DrawCardQuota);

            // Random column
            AddRandomColumnCommand();
        };

        mainGameManager.OnStartTurn_Player_2 += (MainGameContext context) =>
        {
            PlayerContext playerContext = context.PlayerContextByPlayerOrder[2];

            // Start turn animation
            konnekUIManager.StartPlayerTurnAnimation_ClientRpc();

            // Draw card when start turn
            deckManager.DrawCardFromTopDeckServerRpc(
                playerContext.GetClientId()
                , playerContext.DrawCardQuota);

            // Random column
            AddRandomColumnCommand();
        };

    }

    private void KonnekManager_OnKonnekBoardListChanged(NetworkListEvent<Vector3> changeEvent)
    {
        OnKonnekBoardListChanged?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayCard_ServerRpc(ulong cardId, ulong clientId)
    {
        MainGameManager mainGameManager = MainGameManager.Instance;
        Card card = Card.Cache[cardId];

        mainGameManager.AddCommand(new PlayCardCommand(card, clientId));
        OnPlayCardSuccess?.Invoke(mainGameManager.MainGameContext);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyCard_ServerRpc(uint instanceId)
    {
        DestroyCard_ClientRpc(instanceId);
    }

    [ClientRpc]
    private void DestroyCard_ClientRpc(uint instanceId)
    {
        GameObject card = CardHolder.Cache[instanceId];
        Destroy(card);
        CardHolder.Cache.Remove(instanceId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurn_ServerRpc(ulong clientId)
    {
        EndTurn(clientId);
    }

    public void EndTurn(ulong clientId)
    {
        MainGameManager mainGameManager = MainGameManager.Instance;
        MainGameContext mainGameContext = mainGameManager.MainGameContext;
        CommandQueue commandsQueue = mainGameManager.CommandQueue;
        PlayerContext playerContext = mainGameContext.PlayerContextByClientId[clientId];


        // check is player play piece if not then play it
        if (!playerContext.IsPlayedPiece)
        {
            KonnekUIManager.Instance.PlayButton(playerContext.GetClientId());
        }
        
        playerContext.IsPlayedPiece = false;
        // end turn
        Command endTurnCommand = new EndTurnCommand(playerContext.PlayerOrderIndex);
        mainGameManager.AddCommand(endTurnCommand);
        commandsQueue.TryExecuteCommands();
    }


    public void CreateNewBoard()
    {
        InitColumnAmoutClientRpc();
    }

    [ClientRpc]
    private void InitColumnAmoutClientRpc()
    {
        for (int i = 0; i < 7; i++)
        {
            ColumnAmount.Add(i + 1, 0);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayPiece_ServerRpc(int column, ulong clientId)
    {
        MainGameManager mainGameManager = MainGameManager.Instance;
        MainGameContext mainGameContext = mainGameManager.MainGameContext;
        PlayerContext playerContext = mainGameContext.PlayerContextByClientId[clientId];
        if (playerContext.IsPlayedPiece) return;

        Command playCommand = new PlayPieceCommand(column, playerContext.PlayerOrderIndex);
        mainGameManager.AddCommand(playCommand);
        playerContext.IsPlayedPiece = true;
    }

    [ClientRpc]
    public void PlayPiece_ClientRpc(Vector3 playPosition)
    {
        MainGameManager mainGameManager = MainGameManager.Instance;

        Command playCommand = new PlayPieceAnimation(playPosition);
        mainGameManager.AddAnimationCommand(playCommand);
    }

    public bool IsColumnFull(int columnIndex)
    {
        return ColumnAmount[columnIndex] >= 6;
    }
    private void UpdateLocalKonnekBoard()
    {
        localKonnekBoard.Clear();
        foreach (Vector3 positon in KonnekBoard)
        {
            localKonnekBoard.Add(positon);
        }
    }

    public void AddRandomColumnCommand()
    {
        MainGameManager mainGameManager = MainGameManager.Instance;
        MainGameContext mainGameContext = mainGameManager.MainGameContext;
        Command command = new RandomColumnCommand(mainGameContext.PlayColumn);
        mainGameManager.AddCommand(command);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HpDecrease_ServerRpc(ulong clientId, int amount)
    {
        Command hpDecreaseCommand = new DecreaseHpCommand(clientId, amount);
        MainGameManager.Instance.AddCommand(hpDecreaseCommand);
    }
}
