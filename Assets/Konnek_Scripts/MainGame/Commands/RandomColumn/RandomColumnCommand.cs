using System.Collections.Generic;
using UnityEngine;

public class RandomColumnCommand : Command
{
    public const int FIXED_RESULT_REF = -1;
    int fixedResult;
    public RandomColumnCommand(int fixedResult = FIXED_RESULT_REF)
    {
        this.fixedResult = fixedResult;
    }

    public override void Execute()
    {

        int randomColumn;
        KonnekManager konnekManager = KonnekManager.Instance;

        List<int> columnPool = new() { 1, 2, 3, 4, 5, 6, 7 };
        for (int i = 0; i < columnPool.Count; i++)
        {
            if (konnekManager.IsColumnFull(columnPool[i]))
            {
                columnPool.Remove(columnPool[i]);
            }
        }

        if (fixedResult == FIXED_RESULT_REF)
        {
            randomColumn = Random.Range(1, 7);
        }
        else
        {
            randomColumn = fixedResult;
        }

        if (!columnPool.Contains(randomColumn))
        {
            int index = Random.Range(0, columnPool.Count - 1);
            randomColumn = columnPool[index];
        }

        MainGameManager.Instance.MainGameContext.RandomPlayColumn();

        // Play spin wheel animation
        KonnekUIManager.Instance.AddSpinWheelAnimation_ClientRpc(randomColumn);
        base.Execute();
    }

}
