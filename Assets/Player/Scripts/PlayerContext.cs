using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerContext
{
    public string playerName;

    public PlayerContext(PlayerData playerData)
    {
        playerName = playerData.playerName;
    }
}
