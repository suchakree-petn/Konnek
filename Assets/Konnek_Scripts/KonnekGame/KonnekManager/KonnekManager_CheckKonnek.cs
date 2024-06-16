using System.Collections.Generic;
using UnityEngine;

public partial class KonnekManager
{
    public const int MINIMUM_KONNEK = 2;
    public void CheckKonnek(MainGameContext context)
    {
        List<Vector3> playedPositions = PlayedPositions;
        int konnekAmount = 0;
        PlayerContext playerContext = context.GetOpponentPlayerContext();

        int vertical_amount = CheckVertical(playedPositions);
        if (vertical_amount >= MINIMUM_KONNEK)
        {
            if (vertical_amount >= 4)
            {
                vertical_amount *= 2;
            }
            konnekAmount += vertical_amount;
            Debug.Log($"Vertical amount: {vertical_amount}");

            HpDecrease_ServerRpc(playerContext.GetClientId(), vertical_amount);
        }

        int horizontal_amount = CheckHorizontal(playedPositions);
        if (horizontal_amount >= MINIMUM_KONNEK)
        {
            if (horizontal_amount >= 4)
            {
                horizontal_amount *= 2;
            }
            konnekAmount += horizontal_amount;
            Debug.Log($"Horizontal amount: {horizontal_amount}");

            HpDecrease_ServerRpc(playerContext.GetClientId(), horizontal_amount);
        }

        int diagonalSlash_amount = CheckDiagonalSlash(playedPositions);
        if (diagonalSlash_amount >= MINIMUM_KONNEK)
        {
            if (diagonalSlash_amount >= 4)
            {
                diagonalSlash_amount *= 2;
            }
            konnekAmount += diagonalSlash_amount;
            Debug.Log($"Diag Slash amount: {diagonalSlash_amount}");

            HpDecrease_ServerRpc(playerContext.GetClientId(), diagonalSlash_amount);
        }

        int diagonalBSlash_amount = CheckDiagonalBackSlash(playedPositions);
        if (diagonalBSlash_amount >= MINIMUM_KONNEK)
        {
            if (diagonalBSlash_amount >= 4)
            {
                diagonalBSlash_amount *= 2;
            }
            konnekAmount += diagonalBSlash_amount;
            Debug.Log($"Diag Back Slash amount: {diagonalBSlash_amount}");

            HpDecrease_ServerRpc(playerContext.GetClientId(), diagonalBSlash_amount);
        }

    }
    private int CheckVertical(List<Vector3> playedPositions)
    {
        Vector3 lastPosition = playedPositions[^1];

        int countUp = 1;
        for (int i = 1; i < playedPositions.Count; i++)
        {
            if (playedPositions.Contains(new(lastPosition.x, lastPosition.y - i, lastPosition.z)))
            {
                countUp++;
            }
            else
            {
                break;
            }
        }

        int countDown = 1;
        for (int i = 1; i < playedPositions.Count; i++)
        {
            if (playedPositions.Contains(new(lastPosition.x, lastPosition.y + i, lastPosition.z)))
            {
                countDown++;
            }
            else
            {
                break;
            }
        }
        return countUp + countDown -1;
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

    private int CheckDiagonalSlash(List<Vector3> playedPositions)
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


        int sum_slash = countSlashDown + countSlashUp - 1;

        return sum_slash;
    }

    private int CheckDiagonalBackSlash(List<Vector3> playedPositions)
    {
        Vector3 lastPosition = playedPositions[^1];

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

        int sum_BSlash = countBSlashDown + countBSlashUp - 1;

        return sum_BSlash;
    }
}