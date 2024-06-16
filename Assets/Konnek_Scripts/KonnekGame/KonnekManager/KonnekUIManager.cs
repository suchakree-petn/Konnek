using QFSW.QC;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Konnek.SpinWheel;

public class KonnekUIManager : NetworkSingleton<KonnekUIManager>
{
    [Header("Reference")]
    [SerializeField] private Button playPieceButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI playerHpAmountText;
    [SerializeField] private TextMeshProUGUI opponentHpAmountText;

    [Header("Spin Wheel Config")]
    [SerializeField] private SpinWheel spinWheel;
    [SerializeField] private float startDropPositionY;
    [SerializeField] private float endDropPositionY;
    [SerializeField][Range(0f, 5f)] private float dropDuration;
    [SerializeField][Range(0f, 5f)] private float flyDuration;

    private Dictionary<ulong, TextMeshProUGUI> hpTextDict;

    [Header("VFX")]
    [SerializeField] private GameObject HpDecreaseUI;
    [SerializeField] private GameObject startPlayerTurnUI;

    protected override void InitAfterAwake()
    {
        playPieceButton.onClick.AddListener(() => PlayButton(NetworkManager.LocalClientId));
        endTurnButton.onClick.AddListener(() => EndTurnButton(NetworkManager.LocalClientId));
        InitPlayerHpTextDict();
        InitPlayerHpText();


    }



    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

    }

    public void PlayButton(ulong clientId)
    {
        if (MainGameManager.Instance.MainGameContext.IsOwnerTurn(clientId))
        {
            KonnekManager konnekManager = KonnekManager.Instance;
            konnekManager.PlayPiece_ServerRpc(konnekManager.SelectedColumn, clientId);
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
        // Debug.Log($"Client: {clientId} set to {isInteractable}");
        SetPlayPieceButton(isInteractable);
    }

    public void SetPlayPieceButton(bool isInteractable)
    {
        playPieceButton.interactable = isInteractable;
    }

    public void EndTurnButton(ulong clientId)
    {

        MainGameManager mainGameManager = MainGameManager.Instance;
        MainGameContext mainGameContext = mainGameManager.MainGameContext;

        if (mainGameContext.IsOwnerTurn(clientId))
        {
            KonnekManager.Instance.EndTurn_ServerRpc(clientId);

            SetEndTurnButton(false);
        }
    }

    public void StartPlayerTurnAnimation(MainGameContext mainGameContext)
    {
        ulong clientId = mainGameContext.GetCurrentPlayerContext().GetClientId();
        StartPlayerTurnAnimation_ClientRpc(clientId);
    }

    [ClientRpc]
    public void StartPlayerTurnAnimation_ClientRpc(ulong clientId = 0)
    {
        // if (clientId != NetworkManager.LocalClientId) return;
        MainGameManager mainGameManager = MainGameManager.Instance;

        Command startTurnAnimation = new StartTurnAnimation();
        mainGameManager.AddAnimationCommand(startTurnAnimation);
        mainGameManager.AnimationQueue.TryExecuteCommands();
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

    [ClientRpc]
    public void HpDecreasedAnimation_ClientRpc(ulong clientId, int from, int to)
    {
        Command command = new DecreaseHpAnimation(clientId, from, to);
        MainGameManager.Instance.AddAnimationCommand(command);

    }

    private void InitPlayerHpTextDict()
    {
        ulong opponentClientId = default; // Init Hp text dict
        ulong playerClientId = NetworkManager.LocalClientId;

        foreach (PlayerData playerData in KonnekMultiplayerManager.Instance.PlayerDataNetworkList)
        {
            // Debug.Log("Clinet: " + playerData.PlayerName);
            if (playerData.ClientId == playerClientId) continue;

            opponentClientId = playerData.ClientId;
            break;
        }
        hpTextDict = new()
        {
            { playerClientId, playerHpAmountText }, // Player Hp
            { opponentClientId, opponentHpAmountText } // Oppenent Hp
        };
    }

    private void InitPlayerHpText()
    {
        string startHpText = MainGameContext.PLAYER_MAX_HP.ToString();
        foreach (KeyValuePair<ulong, TextMeshProUGUI> clientId in hpTextDict)
        {
            clientId.Value.text = startHpText;
        }
    }

    public void ShowStartPlayerTurnUI()
    {
        bool IsOwnerTurn = MainGameManager.Instance.MainGameContext.IsOwnerTurn(NetworkManager.Singleton.LocalClientId);
        CanvasGroup canvasGroup = startPlayerTurnUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        startPlayerTurnUI.SetActive(true);

        if (IsOwnerTurn)
        {
            canvasGroup.DOFade(1, 0.5f).SetEase(Ease.InSine);
        }
        else
        {
            canvasGroup.DOFade(0, 0.5f).SetEase(Ease.InSine);

        }

        float waitTime = 1f;
        DOTween.To(() => waitTime, x => waitTime = x, 0, 1).OnComplete(() =>
        {
            canvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                startPlayerTurnUI.SetActive(false);
                StartTurnAnimation.OnStartTurnAnimationFinish?.Invoke();
            });
        });
    }

    public void HpDecreasedAnimation(ulong clientId, int startHp, int finishHp)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            GameObject hpDecreaseUI = HpDecreaseUI; // Flashing red screen
            hpDecreaseUI.SetActive(true);
            Image image = hpDecreaseUI.GetComponent<Image>();
            image.DOFade(0, 0.22f).OnComplete(() =>
            {
                hpDecreaseUI.SetActive(false);
                image.DOFade(1, 0);
            });
        }

        Camera.main.DOShakePosition(0.2f, 0.5f, 30, 70); // Shake cameara

        TextMeshProUGUI hpText = hpTextDict[clientId]; // Hp text animation
        float currentHp = startHp;
        DOTween.To(() => currentHp, x => hpText.text = ((int)x).ToString(), finishHp, 0.7f)
        .OnComplete(() =>
        {
            Camera.main.transform.DOMove(new(0, 0, -10), 0);
            DecreaseHpAnimation.OnHpDecreasedAnimationFinish?.Invoke();
        });
    }

    [ClientRpc]
    public void AddSpinWheelAnimation_ClientRpc(int fixedResult = RandomColumnCommand.FIXED_RESULT_REF)
    {
        Command command = new RandomColumnAnimation(fixedResult);
        MainGameManager.Instance.AddAnimationCommand(command);
    }

    #region SpinWheelAnimation
    int result;
    int fixedResult;
    RandomColumnAnimation command;
    public void SpinWheelAnimation(RandomColumnAnimation command, int fixedResult = RandomColumnCommand.FIXED_RESULT_REF)
    {
        this.command = command;
        this.fixedResult = fixedResult;
        GameObject spinWheelGO = spinWheel.gameObject;
        spinWheelGO.SetActive(true);
        Transform spinWheelTransform = spinWheel.transform;

        spinWheelTransform.DOLocalMoveY(endDropPositionY, dropDuration).SetEase(Ease.OutBounce)
        .OnComplete(() =>
        {
            Invoke(nameof(OnStartSpin), 0.5f);
        });
    }
    void OnStartSpin()
    {

        spinWheel.OnSpinEnd((WheelPiece wheelPiece) =>
        {
            result = wheelPiece.Amount;
            Invoke(nameof(OnFinishSpin), 0.5f);
        });

        spinWheel.Spin(fixedResult);
    }

    void OnFinishSpin()
    {
        GameObject spinWheelGO = spinWheel.gameObject;
        Transform spinWheelTransform = spinWheel.transform;
        spinWheelTransform.DOLocalMoveY(startDropPositionY, flyDuration).SetEase(Ease.InBounce)
        .OnComplete(() =>
        {
            spinWheelGO.SetActive(false);
            Invoke(nameof(OnShowResultColumn), 0.5f);

        });
    }

    void OnShowResultColumn()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            KonnekManager.Instance.SelectedColumn = result;
        }
        command.OnRandomColumnAnimationFinish?.Invoke();
    }
    #endregion
}
