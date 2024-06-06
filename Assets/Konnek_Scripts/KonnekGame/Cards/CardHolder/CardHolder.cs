using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class CardHolder : MonoBehaviour
{
    public Card Card;
    public CardState CardState = CardState.Default;
    public bool IsFaceUp;
    public ulong OwnerClientId;
    public uint InstanceId;

    public static Dictionary<uint, GameObject> Cache = new();

    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private Image cardImage;
    [SerializeField] private GameObject frontFace;
    [SerializeField] private GameObject backFace;


    private void Awake()
    {
        OnPlayCardSuccess += HandleOnPlayCardSuccess;
        OnPlayCardFailed += HandleOnPlayCardFailed;
    }

    private void HandleOnPlayCardSuccess()
    {
        KonnekManager.Instance.PlayCard_ServerRpc(Card.cardId);
        KonnekManager.Instance.DestroyCard_ServerRpc(InstanceId);
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
            PlayerHandManager.Instance.SetCardAsChild(card, OwnerClientId);
        })
        .SetEase(Ease.OutSine);

    }

    public void InitCardData(Card card, uint instanceId)
    {
        Card = card;
        cardName.text = card.cardName;
        cardDescription.text = card.cardDescription;
        cardImage.sprite = card.CardSprite;
        InstanceId = instanceId;
        Cache.Add(instanceId, gameObject); // add card gameobject to static dict
        gameObject.name = "Card: " + instanceId.ToString();
    }

    public void SetCardFaceUp()
    {
        IsFaceUp = true;
        UpdateCardFace();
    }

    public void SetCardFaceDown()
    {
        IsFaceUp = false;
        UpdateCardFace();
    }

    public void UpdateCardFace()
    {
        if (IsFaceUp)
        {
            frontFace.SetActive(true);
            backFace.SetActive(false);
        }
        else
        {
            frontFace.SetActive(false);
            backFace.SetActive(true);
        }
    }

    public bool IsOwner(ulong clientId)
    {
        return OwnerClientId == clientId;
    }



#if UNITY_EDITOR
    private void OnValidate()
    {

        UpdateCardFace();

        if (Card != null && IsFaceUp)
        {
            cardName.text = Card.cardName;
            cardDescription.text = Card.cardDescription;
        }
        else
        {
            cardName.text = "Card's name";
            cardDescription.text = "Description";
        }

    }
#endif

}
