using System;
using UnityEngine;

public class PlayPieceAnimation : Command
{
    public Action OnPlayPieceAnimationFinish;

    Vector3 playPosition;
    public PlayPieceAnimation(Vector3 playPosition)
    {
        this.playPosition = playPosition;
    }
    public override void Execute()
    {
        OnPlayPieceAnimationFinish += HandleOnPlayPieceAnimationFinish;
        KonnekManager konnekManager = KonnekManager.Instance;
        KonnekBuilder konnekBuilder = konnekManager.konnekBuilder;

        konnekBuilder.PlayPieceAnimation(playPosition, this);
    }

    private void HandleOnPlayPieceAnimationFinish()
    {
        OnPlayPieceAnimationFinish -= HandleOnPlayPieceAnimationFinish;
        base.Execute();
    }
}
