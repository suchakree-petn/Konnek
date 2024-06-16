using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_MoveLeft", menuName = "Card/Action Card/Move Left")]
public class Card_MoveLeft : Card
{
    public override void PlayCard(ulong clientId)
    {
        int selectedColumn = KonnekManager.Instance.SelectedColumn;
        if (selectedColumn == 1)
        {
            selectedColumn = 7;
        }
        else
        {
            selectedColumn--;
        }
        int countMoveCol = 0;

        while (KonnekManager.Instance.IsColumnFull(selectedColumn))
        {
            if (selectedColumn == 1)
            {
                selectedColumn = 7;
            }

            if (countMoveCol > 9)
            {
                Debug.LogWarning("Error PlayCard: " + cardName + " CardId: " + cardId);
                OnFinishPlayCard?.Invoke();
                return;
            }
            countMoveCol++;
            selectedColumn--;
        }
        KonnekManager.Instance.SelectedColumn = selectedColumn;

        OnFinishPlayCard?.Invoke();

    }


}
