using TMPro;
using UnityEngine;

public partial class CardHolder : MonoBehaviour
{
    public Card Card;
    public CardState cardState = CardState.Default;

    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDescription;


    public void InitCardData(Card card)
    {
        Card = card;
        cardName.text = card.cardName;
        cardDescription.text = card.cardDescription;
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
