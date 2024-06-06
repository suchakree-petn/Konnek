using System;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public partial class CardHolder : IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private DragConfig dragConfig;
    public Action OnPlayCardSuccess;
    public Action OnPlayCardFailed;
    private Vector2 lastPosion;

    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // StartTimer();
        if (!IsOwner(NetworkManager.Singleton.LocalClientId)) return;

        PlayerHandManager.Instance.OnPointerEnterTriggered?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // StopTimer();
        if (!IsOwner(NetworkManager.Singleton.LocalClientId)) return;

        PlayerHandManager.Instance.OnPointerExitTriggered?.Invoke(this);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsOwner(NetworkManager.Singleton.LocalClientId)) return;

        transform.position = eventData.pointerCurrentRaycast.screenPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsOwner(NetworkManager.Singleton.LocalClientId)) return;

        if (eventData.button == PointerEventData.InputButton.Right) return;
        PlayerHandManager.Instance.OnDrag?.Invoke(this);
        GetComponent<Image>().raycastTarget = false;
        lastPosion = transform.localPosition;
        CardState = CardState.Holding;
        transform.SetParent(transform.parent);
        transform.DOMove(eventData.pointerCurrentRaycast.screenPosition, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsOwner(NetworkManager.Singleton.LocalClientId)) return;

        foreach (GameObject go in eventData.hovered)
        {
            if (go.CompareTag("UnplayableArea") ||
            !MainGameManager.Instance.MainGameContext.IsOwnerTurn(NetworkManager.Singleton.LocalClientId))
            {
                OnPlayCardFailed?.Invoke();
                return;
            }
        }
        OnPlayCardSuccess?.Invoke();
    }
}
[Serializable]
public class DragConfig
{
    public float scaleWhileDrag;
}