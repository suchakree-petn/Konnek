using System;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerHand : Singleton<PlayerHand>
{
    public static Action<CardHolder> OnPointerEnterTriggered;
    public static Action<CardHolder> OnPointerExitTriggered;
    public static Action<CardHolder> OnDrag;

    public List<GameObject> cards = new();

    protected override void InitAfterAwake()
    {
    }

    public GameObject GetCardByIndex(int index)
    {
        if (index > transform.childCount || index < 1) return null;
        return transform.GetChild(index).gameObject;
    }
    public GameObject GetLastCard()
    {
        if (transform.childCount == 0)
        {
            return gameObject;
        }
        Transform lastCard = transform.GetChild(transform.childCount - 1);
        return lastCard.gameObject;
    }
    public static void SetCardAsChild(Transform card)
    {
        if (!card.TryGetComponent(out CardHolder _))
        {
            Debug.LogWarning("Only card can be set here");
            return;
        }
        card.SetParent(Instance.transform);
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
