using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckManagerPointerEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public static Action OnDeckClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        OnDeckClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }
    private void OnDestroy() {
        OnDeckClick = null;
    }
}
