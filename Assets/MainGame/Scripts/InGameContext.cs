using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InGameContext
{
    public InGameSetting inGameSetting;
    public PlayerContext playerContext_1;
    public PlayerContext playerContext_2;
    public PlayerContext currentPlayer;

    [Header("Konnek Board")]
    public List<Vector3> playedPositions;

    public InGameContext(PlayerData player1, PlayerData player2, InGameSetting inGameSetting)
    {
        playerContext_1 = new(player1);
        playerContext_2 = new(player2);
        this.inGameSetting = inGameSetting;
        playedPositions = new();
    }
}
