using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
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

    public void ShuffleDeck()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Client can not shuffle decks");
            return;
        }
        cardList = cardList.OrderBy(x => Random.value).ToList();
    }
}
