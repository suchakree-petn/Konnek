using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Deck
{
    [SerializeField] private List<ulong> cardList = new();

    public ulong GetCardByIndex(int index)
    {
        if (cardList.Count == 0)
        {
            return 0;
        }
        ulong card = cardList[index];
        cardList.RemoveAt(index);
        return card;
    }
    public ulong GetCardFromTopDeck()
    {
        if (cardList.Count == 0)
        {
            return 0;
        }
        ulong card = cardList[^1];
        cardList.RemoveAt(cardList.Count - 1);
        return card;
    }
}
