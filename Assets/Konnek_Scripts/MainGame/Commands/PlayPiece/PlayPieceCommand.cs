using UnityEngine;

public class PlayPieceCommand : Command
{
    int column;
    int playerIndex;
    public PlayPieceCommand(int column, int playerIndex)
    {
        this.column = column;
        this.playerIndex = playerIndex;
    }
    public override void Execute()
    {
        KonnekManager konnekManager = KonnekManager.Instance;
        MainGameManager mainGameManager = MainGameManager.Instance;

        konnekManager.ColumnAmount.TryGetValue(column, out int amount);
        if (amount >= 6)
        {
            KonnekManager.OnPlayPieceFailed?.Invoke(mainGameManager.MainGameContext);
            base.Execute();
            return;
        }
        amount++;
        Vector3 playedPosition = new(column, amount, playerIndex);
        konnekManager.KonnekBoard.Add(playedPosition);
        konnekManager.ColumnAmount[column] = amount;
        konnekManager.PlayPiece_ClientRpc(playedPosition);
        KonnekManager.OnPlayPieceSuccess?.Invoke(mainGameManager.MainGameContext);
        base.Execute();
    }

}
