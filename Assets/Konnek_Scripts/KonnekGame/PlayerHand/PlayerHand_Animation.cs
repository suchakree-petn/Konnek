using System;
using DG.Tweening;
using UnityEngine;

public partial class PlayerHand
{
    [Header("Animation Config")]
    [SerializeField] private BumpAnimationConfig bumpAnimationConfig;
    [SerializeField] private PopAnimationConfig popAnimationConfig;



    private void BumpUpCardAnimation()
    {
        // DOTween.Kill(transform);
        // transform.DOScale(bumpUpScale * _originScale, bumpUpDuration)
        // .SetEase(bumpCurve);
        // .OnComplete(ReflectAndSheen);
    }
    private void BumpDownCardAnimation()
    {
        DOTween.Kill(transform);
        // transform.DOScale(_originScale, bumpDownDuration)
        // .SetEase(bumpCurve);
    }

    private void PopUpCardWhileInHandAnimation(CardHolder cardHolder)
    {
        if (cardHolder.cardState != CardState.InHand || cardHolder.IsPopUp) return;
        cardHolder.IsPopUp = true;
        Transform cardHolderTransform = cardHolder.transform.GetChild(0);
        DOTween.Kill(cardHolderTransform);
        cardHolderTransform.localPosition += new Vector3(0, popAnimationConfig.popUpOffset_Y, 0);
        cardHolderTransform.DOScale(popAnimationConfig.popUpScale * 1, popAnimationConfig.popUpDuration)
        .SetEase(popAnimationConfig.popCurve);

    }
    private void PopDownCardWhileInHandAnimation(CardHolder cardHolder)
    {
        if (cardHolder.cardState != CardState.InHand || !cardHolder.IsPopUp) return;
        cardHolder.IsPopUp = false;
        Transform cardHolderTransform = cardHolder.transform.GetChild(0);
        DOTween.Kill(cardHolderTransform);
        cardHolderTransform.localPosition -= new Vector3(0, popAnimationConfig.popUpOffset_Y, 0);
        cardHolderTransform.DOScale(1, popAnimationConfig.popDownDuration)
        .SetEase(popAnimationConfig.popCurve);

    }

}
[Serializable]
public class BumpAnimationConfig
{

    public float bumpUpScale;
    public float bumpUpDuration;
    public float bumpDownDuration;
    public AnimationCurve bumpCurve;
}
[Serializable]
public class PopAnimationConfig
{
    public float popUpOffset_Y;
    public float popUpScale;
    public float popUpDuration;
    public float popDownDuration;
    public AnimationCurve popCurve;
}
