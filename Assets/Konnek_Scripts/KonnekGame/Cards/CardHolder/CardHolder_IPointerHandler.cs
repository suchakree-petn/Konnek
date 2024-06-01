using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
public partial class CardHolder : IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private DragConfig dragConfig;
    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // StartTimer();
        PlayerHand.OnPointerEnterTriggered?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopTimer();
        PlayerHand.OnPointerExitTriggered?.Invoke(this);

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.pointerCurrentRaycast.screenPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right) return;
        PlayerHand.OnDrag?.Invoke(this);
        cardState = CardState.Holding;
        Transform parent = transform.parent;
        transform.SetParent(parent.parent);
        transform.DOScale(dragConfig.scaleWhileDrag, 0);
        transform.DOMove(eventData.pointerCurrentRaycast.screenPosition, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        KonnekManager.Instance.PlayCardServerRpc(Card.cardId);
    }
}
[Serializable]
public class DragConfig
{
    public float scaleWhileDrag;
}