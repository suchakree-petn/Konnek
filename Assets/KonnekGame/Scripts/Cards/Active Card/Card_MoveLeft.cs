using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_MoveLeft", menuName = "Card/Card_MoveLeft")]
public class Card_MoveLeft : Card
{
    public override void PlayCard()
    {
        int selectedColumn = KonnekManager.Instance.SelectedColumn;
        Debug.Log(selectedColumn);
        if (selectedColumn == 1)
        {
            KonnekManager.Instance.SelectedColumn = 7;
        }
        else
        {
            KonnekManager.Instance.SelectedColumn -= 1;
        }
        OnFinishPlayCard?.Invoke();

    }


}
