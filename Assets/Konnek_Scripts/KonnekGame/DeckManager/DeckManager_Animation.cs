using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public partial class DeckManager
{
    [SerializeField] private DeckAnimationConfig deckAnimationConfig;
    internal void DrawAnimation(Transform card, ulong clientId, DrawCardAnimation animation)
    {
        if (card == null)
        {
            return;
        }

        // Prevent from interact with mouse while animation is playing
        Image image = card.GetComponent<Image>();
        image.raycastTarget = false;

        Vector3 spawnPos = PlayerDeckTransformDict[clientId].position;
        card.position = spawnPos;
        GameObject lastCardInHand = PlayerHandManager.Instance.GetLastCard(clientId);
        Vector3 lastCardPosition = lastCardInHand.transform.position;
        card.DOMove(lastCardPosition, deckAnimationConfig.moveToHandDuration)
        .OnComplete(() =>
        {
            image.raycastTarget = true;
            PlayerHandManager.Instance.SetCardAsChild(card, clientId);
            animation.OnFinishDrawCard?.Invoke();
        })
        .SetEase(deckAnimationConfig.moveToHandCurve);
    }
}
[Serializable]
public class DeckAnimationConfig
{
    public float moveToHandDuration;
    public AnimationCurve moveToHandCurve;

}