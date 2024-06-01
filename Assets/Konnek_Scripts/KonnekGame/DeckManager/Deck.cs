using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Deck
{
    [SerializeField] private List<string> cardList = new();

    public string GetCardByIndex(int index)
    {
        if (cardList.Count == 0)
        {
            return null;
        }
        string card = cardList.ElementAt(index);
        cardList.Remove(card);
        return card;
    }
    public string GetCardFromTopDeck()
    {
        if (cardList.Count == 0)
        {
            return null;
        }
        string card = cardList[^1];
        cardList.Remove(card);
        return card;
    }
}
