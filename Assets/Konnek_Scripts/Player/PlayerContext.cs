using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;

[System.Serializable]
public class PlayerContext
{
    public PlayerData playerData;
    public int playerOrderIndex;
    public bool isPlayerTurn;
    public int drawCardQuota;

    public PlayerContext(PlayerData playerData, int playerOrderIndex)
    {
        this.playerData = playerData;
        this.playerOrderIndex = playerOrderIndex;
        isPlayerTurn = playerOrderIndex == 1;
    }

    public ulong GetPlayerId()
    {
        return 999;
    }
}
