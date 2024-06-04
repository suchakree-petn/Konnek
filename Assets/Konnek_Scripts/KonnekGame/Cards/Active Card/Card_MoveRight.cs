using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_MoveRight", menuName = "Card/Action Card/Move Right")]
public class Card_MoveRight : Card
{
    public override void PlayCard()
    {
        int selectedColumn = KonnekManager.Instance.SelectedColumn;
        if (selectedColumn == 7)
        {
            selectedColumn = 1;
        }
        else
        {
            selectedColumn++;
        }
        int countMoveCol = 0;

        while (KonnekManager.Instance.IsColumnFull(selectedColumn))
        {
            if (selectedColumn == 7)
            {
                selectedColumn = 1;
            }

            if (countMoveCol > 9)
            {
                Debug.LogWarning("Error PlayCard: " + cardName + " CardId: " + cardId);
                OnFinishPlayCard?.Invoke();
                return;
            }
            countMoveCol++;
            selectedColumn++;
        }
        KonnekManager.Instance.SelectedColumn = selectedColumn;

        OnFinishPlayCard?.Invoke();

    }


}
