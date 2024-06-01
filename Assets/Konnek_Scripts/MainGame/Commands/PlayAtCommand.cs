using System.Linq;
using UnityEngine;

public class PlayAtCommand : Command
{
    KonnekManager konnekManager;
    int column;
    int playerIndex;
    public PlayAtCommand(int column, int playerIndex)
    {
        konnekManager = KonnekManager.Instance;
        this.column = column;
        this.playerIndex = playerIndex;
    }
    public override void Execute()
    {
        konnekManager.columnAmount.TryGetValue(column, out int amount);
        if (amount >= 6)
        {
            KonnekManager.OnPlayPieceFailed?.Invoke(MainGameManager.Instance.mainGameContext);
            base.Execute();
            return;
        }
        amount++;
        Vector3 playedPosition = new(column, amount, playerIndex);
        konnekManager.konnekBoard.Add(playedPosition);
        konnekManager.columnAmount[column] = amount;
        KonnekManager.OnPlayPieceSuccess?.Invoke(MainGameManager.Instance.mainGameContext);
        KonnekBuilder.OnCompleteDropAnimation += base.Execute;
    }

}
