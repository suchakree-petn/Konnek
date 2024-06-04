using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class CardHolder : MonoBehaviour
{
    public Card Card;
    public CardState CardState = CardState.Default;

    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private Image cardImage;


    private void Awake()
    {
        OnPlayCardSuccess += HandleOnPlayCardSuccess;
        OnPlayCardFailed += HandleOnPlayCardFailed;
    }

    private void HandleOnPlayCardSuccess()
    {
        KonnekManager.Instance.PlayCardServerRpc(Card.cardId);
        Destroy(gameObject);
    }

    private void HandleOnPlayCardFailed()
    {
        ReturnCardToPlayerHand();
    }

    private void ReturnCardToPlayerHand()
    {
        Transform card = transform;

        // Prevent from interact with mouse while animation is playing
        Image image = card.GetComponent<Image>();
        image.raycastTarget = false;

        Vector3 lastCardPosition = lastPosion;
        CardHolder cardHolder = card.GetComponent<CardHolder>();
        cardHolder.CardState = CardState.InHand;
        PlayerHandManager.Instance.PopDownCardWhileInHandAnimation(cardHolder);
        card.DOLocalMove(lastCardPosition, 0.3f)
        .OnComplete(() =>
        {
            image.raycastTarget = true;
            PlayerHandManager.SetCardAsChild(card);
        })
        .SetEase(Ease.OutSine);

    }

    public void InitCardData(Card card)
    {
        Card = card;
        cardName.text = card.cardName;
        cardDescription.text = card.cardDescription;
        cardImage.sprite = card.CardSprite;
    }
    private void OnValidate()
    {
#if UNITY_EDITOR

        if (Card != null)
        {
            cardName.text = Card.cardName;
            cardDescription.text = Card.cardDescription;
        }
        else
        {
            cardName.text = "Card's name";
            cardDescription.text = "Description";
        }

#endif

    }

}
