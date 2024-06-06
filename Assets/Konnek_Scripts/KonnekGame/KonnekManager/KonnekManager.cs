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
    private static uint cardInstanceId = 0;

    public static uint CardInstanceIdPointer
    {
        get
        {
            uint currentId = cardInstanceId;
            cardInstanceId++;
            return currentId;
        }
    }

    protected override void InitAfterAwake()
    {
        playerActions = new();
        playerActions.InGame.Enable();
        KonnekBoard = new NetworkList<Vector3>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        KonnekBoard.OnListChanged += KonnekManager_OnKonnekBoardListChanged;
        OnKonnekBoardListChanged += UpdateLocalKonnekBoard;


    }



    public override void OnNetworkSpawn()
    {
        playerActions.InGame.SelectLeftColumn.performed += (ctx) => RequestMoveColumnServerRpc(SelectedColumn - 1);
        playerActions.InGame.SelectRightColumn.performed += (ctx) => RequestMoveColumnServerRpc(SelectedColumn + 1);

        if (!IsServer) return;
        CreateNewBoard();
        OnPlayPieceSuccess += CheckKonnek;


        MainGameManager.Instance.OnGameStart += (MainGameContext context) =>
        {
            OnPlayPieceFailed += (ctx) => Debug.Log("Cant play this column"); ;
            OnInitPlayerFinish += (ctx) => MainGameManager.Instance.SetCurrentGameState(MainGameState.Player_1_Start_Turn);

            DeckManager.Instance.DrawCardFromTopDeckServerRpc(
                context.PlayerContextByPlayerOrder[1].GetClientId()
                , MainGameContext.START_CARD_AMOUNT);

            DeckManager.Instance.DrawCardFromTopDeckServerRpc(
                context.PlayerContextByPlayerOrder[2].GetClientId()
                , MainGameContext.START_CARD_AMOUNT);

            OnInitPlayerFinish?.Invoke(context); // Ready to start player 1 turn
        };
        MainGameManager.Instance.OnStartTurn_Player_1 += (MainGameContext context) =>
        {
            PlayerContext playerContext = context.PlayerContextByPlayerOrder[1]; 
            
            // Set draw card quota when start turn
            int drawAmount = MainGameContext.DRAW_CARD_QUOTA_AMOUNT + playerContext.BonusDrawCardQuota;
            playerContext.DrawCardQuota = drawAmount;

            // Draw card when start turn
            DeckManager.Instance.DrawCardFromTopDeckServerRpc( 
                playerContext.GetClientId()
                , playerContext.DrawCardQuota);

            playerContext.DrawCardQuota -= drawAmount;
        };

        MainGameManager.Instance.OnStartTurn_Player_2 += (MainGameContext context) =>
        {
            PlayerContext playerContext = context.PlayerContextByPlayerOrder[2]; 

            // Set draw card quota when start turn
            int drawAmount = MainGameContext.DRAW_CARD_QUOTA_AMOUNT + playerContext.BonusDrawCardQuota;
            
            playerContext.DrawCardQuota = drawAmount;
            // Draw card when start turn
            DeckManager.Instance.DrawCardFromTopDeckServerRpc(
                playerContext.GetClientId()
                , playerContext.DrawCardQuota);

            playerContext.DrawCardQuota -= drawAmount;

        };

    }

    private void KonnekManager_OnKonnekBoardListChanged(NetworkListEvent<Vector3> changeEvent)
    {
        OnKonnekBoardListChanged?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayCard_ServerRpc(ulong cardId)
    {
        Card card = Card.Cache[cardId];
        MainGameManager.Instance.AddCommand(new PlayCardCommand(card));
        OnPlayCardSuccess?.Invoke(MainGameManager.Instance.MainGameContext);
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
        MainGameManager mainGameManager = MainGameManager.Instance;

        // check is player play piece if not then play it
        MainGameContext mainGameContext = mainGameManager.MainGameContext;
        if (!mainGameContext.PlayerContextByClientId[clientId].IsPlayedPiece)
        {
            KonnekUIManager.Instance.PlayButton(clientId);
        }
        mainGameContext.PlayerContextByClientId[clientId].IsPlayedPiece = false;

        // check is all command execute before end turn
        CommandQueue commandsQueue = mainGameManager.CommandQueue;
        if (commandsQueue.commandsQueue.Count > 0)
        {
            mainGameManager.SetCurrentGameState(MainGameState.Idle);
            mainGameContext.GetCurrentPlayerContext().IsPlayerTurn = false;
        }

        // end turn
        Command endTurnCommand = new EndTurnCommand();
        commandsQueue.AddCommand(endTurnCommand);
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
    public void PlayAtServerRpc(int column, ulong clientId)
    {
        if (MainGameManager.Instance.MainGameContext.GetCurrentPlayerContext().IsPlayedPiece) return;

        Command playCommand = new PlayAtCommand(column,
            MainGameManager.Instance.MainGameContext.GetCurrentPlayerContext().PlayerOrderIndex);
        MainGameManager.Instance.CommandQueue.AddCommand(playCommand);
        MainGameManager.Instance.MainGameContext.PlayerContextByClientId[clientId].IsPlayedPiece = true;
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

}
