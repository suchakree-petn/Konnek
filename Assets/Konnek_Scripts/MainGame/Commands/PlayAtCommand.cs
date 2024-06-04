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
        konnekManager.ColumnAmount.TryGetValue(column, out int amount);
        if (amount >= 6)
        {
            KonnekManager.OnPlayPieceFailed?.Invoke(MainGameManager.Instance.MainGameContext);
            base.Execute();
            return;
        }
        amount++;
        Vector3 playedPosition = new(column, amount, playerIndex);
        konnekManager.KonnekBoard.Add(playedPosition);
        konnekManager.ColumnAmount[column] = amount;
        KonnekManager.OnPlayPieceSuccess?.Invoke(MainGameManager.Instance.MainGameContext);
        KonnekBuilder.OnCompleteDropAnimation += base.Execute;
    }

}
