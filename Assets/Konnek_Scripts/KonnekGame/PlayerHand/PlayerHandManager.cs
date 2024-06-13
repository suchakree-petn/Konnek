using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class PlayerHandManager : NetworkSingleton<PlayerHandManager>
{
    public Action<CardHolder> OnPointerEnterTriggered;
    public Action<CardHolder> OnPointerExitTriggered;
    public Action<CardHolder> OnDrag;

    public List<GameObject> cards = new();

    public CardHolder CurrentHoldingCard;


    [Header("Reference")]
    [SerializeField] private Transform playerHandTransform;
    [SerializeField] private Transform opponentHandTransform;
    public Dictionary<ulong, Transform> PlayerHandTransformDict = new();


    protected override void InitAfterAwake()
    {
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        MainGameContext mainGameContext = MainGameManager.Instance.MainGameContext;
        InitPlayerHandTransform_ClientRpc(
            mainGameContext.GetPlayerContextByIndex(1).GetClientId()
            , mainGameContext.GetPlayerContextByIndex(2).GetClientId());

    }


    [ClientRpc]
    private void InitPlayerHandTransform_ClientRpc(ulong clientId_1, ulong clientId_2)
    {
        if (NetworkManager.LocalClientId != clientId_1)
        {
            PlayerHandTransformDict.Add(clientId_2, playerHandTransform);
            PlayerHandTransformDict.Add(clientId_1, opponentHandTransform);

        }
        else
        {
            PlayerHandTransformDict.Add(clientId_1, playerHandTransform);
            PlayerHandTransformDict.Add(clientId_2, opponentHandTransform);
        }
    }
    public GameObject GetCardByIndex(int index)
    {
        if (index > playerHandTransform.childCount || index < 1) return null;
        return playerHandTransform.GetChild(index).gameObject;
    }
    public GameObject GetLastCard(ulong clientId)
    {
        Transform playerHandTransform = PlayerHandTransformDict[clientId];
        if (playerHandTransform.childCount == 0)
        {
            return playerHandTransform.gameObject;
        }

        Transform lastCard = playerHandTransform.GetChild(playerHandTransform.childCount - 1);
        return lastCard.gameObject;
    }
    public void SetCardAsChild(Transform card, ulong clientId)
    {
        if (!card.TryGetComponent(out CardHolder _))
        {
            Debug.LogWarning("Only card can be set here");
            return;
        }
        Transform parent = PlayerHandTransformDict[clientId];
        card.SetParent(parent);
    }
    private void OnEnable()
    {
        // OnPointerEnterTriggered += BumpUpCardAnimation;
        // OnPointerExitTriggered += BumpDownCardAnimation;
        OnPointerEnterTriggered += PopUpCardWhileInHandAnimation;
        OnPointerExitTriggered += PopDownCardWhileInHandAnimation;
        OnDrag += PopDownCardWhileInHandAnimation;
    }
    private void OnDisable()
    {
        // OnPointerEnterTriggered -= BumpUpCardAnimation;
        // OnPointerExitTriggered -= BumpDownCardAnimation;
        OnPointerEnterTriggered -= PopUpCardWhileInHandAnimation;
        OnPointerExitTriggered -= PopDownCardWhileInHandAnimation;
        OnDrag -= PopDownCardWhileInHandAnimation;

    }


}
