using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KonnekBoard
{
    public List<Vector3> board;
    public Dictionary<int, int> columnAmount;

    public KonnekBoard()
    {
        board = new(42);
        columnAmount = new Dictionary<int, int>();
    }
}
