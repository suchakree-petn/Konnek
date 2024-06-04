using System;
using System.Collections.Generic;
using DG.Tweening;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public partial class KonnekManager : NetworkSingleton<KonnekManager>
{
    public delegate void KonnekDelegate(MainGameContext context);
    public static KonnekDelegate OnPlayPieceSuccess;
    public static KonnekDelegate OnPlayPieceFailed;
    public static KonnekDelegate OnPlayCardSuccess;
    public static KonnekDelegate OnPlayCardFailed;
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

    // [Header("Player HP")]

    [Header("Button")]
    [SerializeField] private Button playPieceButton;
    [SerializeField] private Button endTurnButton;

    [Header("VFX")]
    [SerializeField] public GameObject HpDecreaseUI;

    protected override void InitAfterAwake()
    {
        playerActions = new();
        playerActions.InGame.Enable();
        KonnekBoard = new NetworkList<Vector3>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        KonnekBoard.OnListChanged += KonnekManager_OnKonnekBoardListChanged;
        OnKonnekBoardListChanged += UpdateLocalKonnekBoard;

        playPieceButton.onClick.AddListener(() => PlayButton(NetworkManager.LocalClientId));
        endTurnButton.onClick.AddListener(() => EndTurnButton(NetworkManager.LocalClientId));
    }



    public override void OnNetworkSpawn()
    {
        playerActions.InGame.SelectLeftColumn.performed += (ctx) => RequestMoveColumnServerRpc(SelectedColumn - 1);
        playerActions.InGame.SelectRightColumn.performed += (ctx) => RequestMoveColumnServerRpc(SelectedColumn + 1);
        // playerActions.InGame.PlayPiece.performed += (ctx) => PlayAtServerRpc(SelectedColumn);

        if (!IsServer) return;
        CreateNewBoard();
        OnPlayPieceSuccess += CheckKonnek;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HpDecrease_ServerRpc(NetworkManager.LocalClientId, 5);
        }
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
    private void PlayButton(ulong clientId)
    {
        if (MainGameManager.Instance.MainGameContext.IsOwnerTurn(clientId))
        {
            PlayAtServerRpc(SelectedColumn, clientId);
            SetPlayPieceButton_ServerRpc(false, clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayPieceButton_ServerRpc(bool isInteractable, ulong clientId)
    {
        SetPlayPieceButton_ClientRpc(isInteractable, clientId);
    }

    [ClientRpc]
    public void SetPlayPieceButton_ClientRpc(bool isInteractable, ulong clientId)
    {
        if (clientId != NetworkManager.LocalClientId) return;
        Debug.Log($"Client: {clientId} set to {isInteractable}");
        SetPlayPieceButton(isInteractable);
    }

    public void SetPlayPieceButton(bool isInteractable)
    {
        playPieceButton.interactable = isInteractable;
    }

    public void EndTurnButton(ulong clientId)
    {
        MainGameContext mainGameContext = MainGameManager.Instance.MainGameContext;
        if (mainGameContext.IsOwnerTurn(clientId))
        {
            EndTurn_ServerRpc(clientId);
            SetEndTurnButton_ServerRpc(false, clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurn_ServerRpc(ulong clientId)
    {
        MainGameContext mainGameContext = MainGameManager.Instance.MainGameContext;
        if (!mainGameContext.PlayerContextByClientId[clientId].IsPlayedPiece)
        {
            PlayButton(clientId);
        }
        mainGameContext.PlayerContextByClientId[clientId].IsPlayedPiece = false;
        if (MainGameManager.Instance.CommandQueue.commandsQueue.Count > 0)
        {
            MainGameManager.Instance.MainGameContext.SetCurrentGameState(MainGameState.Idle);
            MainGameManager.Instance.MainGameContext.GetCurrentPlayerContext().IsPlayerTurn = false;
        }
        Command endTurnCommand = new EndTurnCommand();
        MainGameManager.Instance.CommandQueue.AddCommand(endTurnCommand);
        MainGameManager.Instance.CommandQueue.TryExecuteCommandsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetEndTurnButton_ServerRpc(bool isInteractable, ulong clientId)
    {
        SetEndTurnButton_ClientRpc(isInteractable, clientId);
    }

    [ClientRpc]
    public void SetEndTurnButton_ClientRpc(bool isInteractable, ulong clientId)
    {
        if (clientId != NetworkManager.LocalClientId) return;
        SetEndTurnButton(isInteractable);
    }

    public void SetEndTurnButton(bool isInteractable)
    {
        endTurnButton.interactable = isInteractable;
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
    private void PlayAtServerRpc(int column, ulong clientId)
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

    [ServerRpc(RequireOwnership = false), Command]
    public void HpDecrease_ServerRpc(ulong clientId, int amount)
    {
        Command hpDecreaseCommand = new DecreaseHPCommand(clientId, amount);
        MainGameManager.Instance.AddCommand(hpDecreaseCommand);

    }

    [ClientRpc]
    public void HpDecreasedAnimation_ClientRpc(ulong clientId)
    {
        Debug.Log("Here " + clientId);
        Debug.Log("Here " + NetworkManager.Singleton.LocalClientId);
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        Camera.main.DOShakePosition(0.2f, 1);
        GameObject hpDecreaseUI = HpDecreaseUI;
        hpDecreaseUI.SetActive(true);

        Image image = hpDecreaseUI.GetComponent<Image>();
        image.DOFade(0, 0.22f).OnComplete(() =>
        {
            hpDecreaseUI.SetActive(false);
            image.DOFade(1, 0);
            HandleHpDecreasedAnimation_ServerRpc();
        });
    }

    [ServerRpc(RequireOwnership = false)]
    public void HandleHpDecreasedAnimation_ServerRpc()
    {
        DecreaseHPCommand.OnHpDecreasedAnimationFinish?.Invoke();
    }
}
