using System.Collections.Generic;
using UnityEngine;

public partial class KonnekManager
{
    public void CheckKonnek(MainGameContext context)
    {
        List<Vector3> playedPositions = this.playedPositions;

        int konnekAmount = 0;

        int vertical_amount = CheckVertical(playedPositions);
        if (vertical_amount >= 4)
        {
            konnekAmount += vertical_amount;
        }

        int horizontal_amount = CheckHorizontal(playedPositions);
        if (horizontal_amount >= 4)
        {
            konnekAmount += horizontal_amount;
        }

        int diagonal_amount = CheckDiagonal(playedPositions);
        if (diagonal_amount >= 4)
        {
            konnekAmount += diagonal_amount;
        }

        Debug.Log($"Konnek amount: {konnekAmount}");
    }
    private int CheckVertical(List<Vector3> playedPositions)
    {
        Vector3 lastPosition = playedPositions[^1];
        int count = 1;

        for (int i = 1; i < playedPositions.Count; i++)
        {
            if (playedPositions.Contains(new(lastPosition.x, lastPosition.y - i, lastPosition.z)))
            {
                count++;
            }
            else
            {
                break;
            }
        }
        // Debug.Log($"Vertical: {count}");
        return count;
    }
    private int CheckHorizontal(List<Vector3> playedPositions)
    {
        Vector3 lastPosition = playedPositions[^1];

        int countLeft = 1;
        // Left check
        for (int i = 1; i < playedPositions.Count; i++)
        {
            if (playedPositions.Contains(new(lastPosition.x - i, lastPosition.y, lastPosition.z)))
            {
                countLeft++;
            }
            else
            {
                break;
            }
        }

        int countRight = 1;
        // Right check
        for (int i = 1; i < playedPositions.Count; i++)
        {
            if (playedPositions.Contains(new(lastPosition.x + i, lastPosition.y, lastPosition.z)))
            {
                countRight++;
            }
            else
            {
                break;
            }
        }

        int sum = countLeft + countRight - 1;
        // Debug.Log($"Horizontal: {sum}");

        return sum;
    }

    private int CheckDiagonal(List<Vector3> playedPositions)
    {
        Vector3 lastPosition = playedPositions[^1];

        int countSlashDown = 1;
        // Check / down
        for (int i = 1; i < playedPositions.Count; i++)
        {
            if (playedPositions.Contains(new(lastPosition.x - i, lastPosition.y - i, lastPosition.z)))
            {
                countSlashDown++;
            }
            else
            {
                break;
            }
        }

        int countSlashUp = 1;
        // Check / up
        for (int i = 1; i < playedPositions.Count; i++)
        {
            if (playedPositions.Contains(new(lastPosition.x + i, lastPosition.y + i, lastPosition.z)))
            {
                countSlashUp++;
            }
            else
            {
                break;
            }
        }

        int countBSlashDown = 1;
        // Check \ down
        for (int i = 1; i < playedPositions.Count; i++)
        {
            if (playedPositions.Contains(new(lastPosition.x + i, lastPosition.y - i, lastPosition.z)))
            {
                countBSlashDown++;
            }
            else
            {
                break;
            }
        }

        int countBSlashUp = 1;
        // Check \ up
        for (int i = 1; i < playedPositions.Count; i++)
        {
            if (playedPositions.Contains(new(lastPosition.x - i, lastPosition.y + i, lastPosition.z)))
            {
                countBSlashUp++;
            }
            else
            {
                break;
            }
        }

        int sum_slash = countSlashDown + countSlashUp - 1;
        int sum_BSlash = countBSlashDown + countBSlashUp - 1;
        // Debug.Log($"Slash: {sum_slash}");
        // Debug.Log($"Back Slash: {sum_BSlash}");
        if (sum_slash < 4)
        {
            sum_slash = 1;
        }
        if (sum_BSlash < 4)
        {
            sum_BSlash = 1;
        }

        return sum_slash + sum_BSlash - 1;
    }
}