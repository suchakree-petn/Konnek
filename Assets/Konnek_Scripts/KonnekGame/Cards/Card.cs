using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    public Action OnFinishPlayCard;

    [Header("Info")]
    public ulong cardId;
    public string cardName;
    [TextArea(1, 30)]
    public string cardDescription;
    public CardType cardType;

    [Header("Reference")]
    public Sprite CardSprite;

    static Dictionary<ulong, Card> _cache;
    public static Dictionary<ulong, Card> Cache
    {
        get
        {
            if (_cache == null)
            {
                Card[] cards = Resources.LoadAll<Card>("KonnekGame/Cards");
                _cache = cards.ToDictionary(card => card.cardId, card => card);
            }
            return _cache;
        }
    }

    public abstract void PlayCard();
    public virtual void Discard()
    {

    }
}
