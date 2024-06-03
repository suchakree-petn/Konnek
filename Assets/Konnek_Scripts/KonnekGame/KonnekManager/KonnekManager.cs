using System;
using System.Collections.Generic;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

public partial class KonnekManager : NetworkSingleton<KonnekManager>
{
    public delegate void KonnekDelegate(MainGameContext context);
    public static KonnekDelegate OnPlayPieceSuccess;
    public static KonnekDelegate OnPlayPieceFailed;
    public static KonnekDelegate OnPlayCardSuccess;
    public static KonnekDelegate OnPlayCardFailed;
    public Action OnKonnekBoardListChanged;

    [Header("Konnek Board")]
    public NetworkList<Vector3> konnekBoard;
    public Dictionary<int, int> columnAmount = new();
    [SerializeField] private List<Vector3> localKonnekBoard = new();
    public List<Vector3> playedPositions
    {
        get
        {
            List<Vector3> temp = new();
            foreach (var item in konnekBoard)
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
    protected override void InitAfterAwake()
    {
        playerActions = new();
        playerActions.InGame.Enable();
        konnekBoard = new NetworkList<Vector3>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        konnekBoard.OnListChanged += KonnekManager_OnKonnekBoardListChanged;
        OnKonnekBoardListChanged += UpdateLocalKonnekBoard;

    }

    public override void OnNetworkSpawn()
    {
        playerActions.InGame.SelectLeftColumn.performed += (ctx) => RequestMoveColumnServerRpc(SelectedColumn - 1);
        playerActions.InGame.SelectRightColumn.performed += (ctx) => RequestMoveColumnServerRpc(SelectedColumn + 1);
        playerActions.InGame.PlayPiece.performed += (ctx) => PlayAtServerRpc(SelectedColumn);

        if (!IsServer) return;
        CreateNewBoard();
        OnPlayPieceSuccess += CheckKonnek;
    }

    private void KonnekManager_OnKonnekBoardListChanged(NetworkListEvent<Vector3> changeEvent)
    {
        OnKonnekBoardListChanged?.Invoke();
    }
    [ServerRpc(RequireOwnership = false)]
    public void PlayCardServerRpc(ulong cardId)
    {
        Card card = Card.Cache[cardId];
        MainGameManager.Instance.AddCommand(new PlayCardCommand(card));
        OnPlayCardSuccess?.Invoke(MainGameManager.Instance.MainGameContext);
    }

    private void UpdateLocalKonnekBoard()
    {
        localKonnekBoard.Clear();
        foreach (Vector3 positon in konnekBoard)
        {
            localKonnekBoard.Add(positon);
        }
        // switch (changeEvent.Type)
        // {
        //     case NetworkListEvent<Vector3>.EventType.Add:
        //         localKonnekBoard.Add(changeEvent.Value);
        //         break;
        //     case NetworkListEvent<Vector3>.EventType.Value:
        //         localKonnekBoard[changeEvent.Index] = changeEvent.Value;
        //         break;
        //     case NetworkListEvent<Vector3>.EventType.Full:
        //         localKonnekBoard.Clear();
        //         foreach (Vector3 positon in konnekBoard)
        //         {
        //             localKonnekBoard.Add(positon);
        //         }
        //         break;
        //     case NetworkListEvent<Vector3>.EventType.Insert:
        //         localKonnekBoard.Insert(changeEvent.Index, changeEvent.Value);
        //         break;
        //     case NetworkListEvent<Vector3>.EventType.RemoveAt:
        //         localKonnekBoard.RemoveAt(changeEvent.Index);
        //         break;
        //     case NetworkListEvent<Vector3>.EventType.Remove:
        //         localKonnekBoard.Remove(changeEvent.Value);
        //         break;
        //     case NetworkListEvent<Vector3>.EventType.Clear:
        //         localKonnekBoard.Clear();
        //         break;
        //     default:
        //         break;
        // }

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
            columnAmount.Add(i + 1, 0);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayAtServerRpc(int column, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        if (!MainGameManager.Instance.MainGameContext.IsOwnerTurn(clientId)) return;

        Command playCommand = new PlayAtCommand(column,
            MainGameManager.Instance.MainGameContext.GetCurrentPlayerContext().PlayerOrderIndex);
        MainGameManager.Instance.CommandQueue.AddCommand(playCommand);
    }




}
