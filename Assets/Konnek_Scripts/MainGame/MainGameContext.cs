using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class MainGameContext
{
    private readonly MainGameSetting mainGameSetting;
    [SerializeField] private PlayerContext playerContext_1;
    [SerializeField] private PlayerContext playerContext_2;
    [SerializeField] private PlayerContext currentPlayer;
    // private Dictionary<ulong, PlayerContext> playerContext;
    // public Dictionary<ulong, PlayerContext> PlayerContext
    // {
    //     get{
    //         if(playerContext ==null){
    //     playerContext = new();
    //     playerContext.Add()
    //         }
    //     }
    // }
    public MainGameState currentState;
    public float TurnDuration => mainGameSetting.turnDuration;
    public float currentTurnDuration;

    public MainGameContext(PlayerData player1, PlayerData player2, MainGameSetting mainGameSetting)
    {
        playerContext_1 = new(player1, 1);
        playerContext_2 = new(player2, 2);
        currentPlayer = playerContext_1;
        this.mainGameSetting = mainGameSetting;
        currentState = MainGameState.Default;
        currentTurnDuration = mainGameSetting.turnDuration;
    }


    public PlayerContext GetPlayerContext(int playerIndex)
    {
        if (playerIndex == 1)
        {
            return playerContext_1;
        }
        return playerContext_2;
    }
    public PlayerContext GetPlayerContextByPlayerId(ulong playerId)
    {
        if (playerId == playerContext_1.GetPlayerId())
        {
            return playerContext_1;
        }
        return playerContext_2;
    }
    public PlayerContext GetPlayerContextByClientId(ulong clientId)
    {
        if (clientId == 0)
        {
            return playerContext_1;
        }
        return playerContext_2;
    }
    public PlayerContext GetCurrentPlayerContext()
    {
        if (currentPlayer.playerOrderIndex == 1)
        {
            return playerContext_1;
        }
        return playerContext_2;
    }
    public PlayerContext GetOtherPlayerContext()
    {
        if (currentPlayer.playerOrderIndex == 2)
        {
            return playerContext_1;
        }
        return playerContext_2;
    }
    public PlayerContext GetOwnerPlayerContext()
    {
        return GetPlayerContextByPlayerId(0);
    }
    public void SetCurrentPlayerContext(int playerIndex)
    {
        PlayerContext currentPlayer = GetPlayerContext(playerIndex);
        this.currentPlayer = currentPlayer;
    }
    public bool IsOwnerTurn(ulong clientId)
    {
        return currentPlayer.playerOrderIndex - 1 == (int)clientId;
    }
    public int GetPlayerIndexByPlayerId(ulong playerId)
    {
        if (playerId == playerContext_1.GetPlayerId())
        {
            return playerContext_1.playerOrderIndex;
        }
        else if (playerId == playerContext_2.GetPlayerId())
        {
            return playerContext_2.playerOrderIndex;
        }
        return -999;
    }
    public int GetPlayerIndexByClientId(ulong clientId)
    {
        if ((int)clientId == playerContext_1.playerOrderIndex - 1)
        {
            return playerContext_1.playerOrderIndex;
        }
        else if ((int)clientId == playerContext_2.playerOrderIndex - 1)
        {
            return playerContext_2.playerOrderIndex;
        }
        return -999;
    }
    public bool CanDraw(ulong clientId)
    {
        if (IsOwnerTurn(clientId) && GetOwnerPlayerContext().drawCardQuota > 0)
        {
            return true;
        }
        Debug.Log($"Cant draw\nIs owner turn: {IsOwnerTurn(clientId)}\nDraw card quota: {GetOwnerPlayerContext().drawCardQuota}");
        return false;
    }

}

