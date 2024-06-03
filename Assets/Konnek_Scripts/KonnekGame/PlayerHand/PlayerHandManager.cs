using System;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerHandManager : Singleton<PlayerHandManager>
{
    public Action<CardHolder> OnPointerEnterTriggered;
    public Action<CardHolder> OnPointerExitTriggered;
    public Action<CardHolder> OnDrag;

    public List<GameObject> cards = new();

    public CardHolder CurrentHoldingCard;

    [SerializeField] private GameObject playerHandTransform;

    protected override void InitAfterAwake()
    {
    }

    public GameObject GetCardByIndex(int index)
    {
        if (index > playerHandTransform.transform.childCount || index < 1) return null;
        return playerHandTransform.transform.GetChild(index).gameObject;
    }
    public GameObject GetLastCard()
    {
        if (playerHandTransform.transform.childCount == 0)
        {
            return playerHandTransform;
        }
        Transform lastCard = playerHandTransform.transform.GetChild(playerHandTransform.transform.childCount - 1);
        return lastCard.gameObject;
    }
    public static void SetCardAsChild(Transform card)
    {
        if (!card.TryGetComponent(out CardHolder _))
        {
            Debug.LogWarning("Only card can be set here");
            return;
        }
        card.SetParent(Instance.playerHandTransform.transform);
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
