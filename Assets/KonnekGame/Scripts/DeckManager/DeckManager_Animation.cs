using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
public partial class DeckManager
{
    [SerializeField] private DeckAnimationConfig deckAnimationConfig;
    internal void DrawAnimation(Transform card, int deckIndex)
    {
        if (card == null)
        {
            return;
        }

        // Prevent from interact with mouse while animation is playing
        Image image = card.GetComponent<Image>();
        image.raycastTarget = false;

        Vector3 spawnPos = deckAnimationConfig.GetDeckTransformByIndex(deckIndex).position;
        card.position = spawnPos;
        GameObject lastCardInHand = PlayerHand.Instance.GetLastCard();
        Vector3 lastCardPosition;
        lastCardPosition = lastCardInHand.transform.position;
        card.DOMove(lastCardPosition, deckAnimationConfig.moveToHandDuration)
        .OnComplete(() =>
        {
            image.raycastTarget = true;
            PlayerHand.SetCardAsChild(card);
        })
        .SetEase(deckAnimationConfig.moveToHandCurve);
    }

}
[Serializable]
public class DeckAnimationConfig
{
    public float moveToHandDuration;
    public AnimationCurve moveToHandCurve;
    public Transform deckTransform_1;
    public Transform deckTransform_2;

    public Transform GetDeckTransformByIndex(int index)
    {
        return index == 1 ? deckTransform_1 : deckTransform_2;
    }
}