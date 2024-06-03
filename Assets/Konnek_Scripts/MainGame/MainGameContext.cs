using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class MainGameContext
{
    private readonly MainGameSetting mainGameSetting;
    [SerializeField] private PlayerContext playerContext_1;
    [SerializeField] private PlayerContext playerContext_2;
    [SerializeField] private PlayerContext currentPlayer;
    public Dictionary<ulong, PlayerContext> PlayerContextByClientId
    {
        get
        {
            Dictionary<ulong, PlayerContext> result = new()
            {
                {playerContext_1.GetClientId(),playerContext_1},
                {playerContext_2.GetClientId(),playerContext_2}
            }; 
            return result;
        }
    }

    public Dictionary<int, PlayerContext> PlayerContextByPlayerOrder
    {
        get
        {
            Dictionary<int, PlayerContext> result  = new()
            {
                {playerContext_1.PlayerOrderIndex,playerContext_1},
                {playerContext_2.PlayerOrderIndex,playerContext_2}
            }; 
            return result;
        }
    }

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
        if (playerId == playerContext_1.GetClientId())
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
        if (currentPlayer.PlayerOrderIndex == 1)
        {
            return playerContext_1;
        }
        return playerContext_2;
    }
    public PlayerContext GetOtherPlayerContext()
    {
        if (currentPlayer.PlayerOrderIndex == 2)
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
        return MainGameManager.Instance.CurrentClientTurn.Value == clientId;
    }
    public int GetPlayerIndexByClientId(ulong clientId)
    {
        if ((int)clientId == playerContext_1.PlayerOrderIndex - 1)
        {
            return playerContext_1.PlayerOrderIndex;
        }
        else if ((int)clientId == playerContext_2.PlayerOrderIndex - 1)
        {
            return playerContext_2.PlayerOrderIndex;
        }
        return -999;
    }
    public bool CanDraw(ulong clientId)
    {
        if (IsOwnerTurn(clientId) && GetCurrentPlayerContext().DrawCardQuota > 0)
        {
            return true;
        }
        Debug.Log($"Cant draw\nIs owner turn: {IsOwnerTurn(clientId)}\nDraw card quota: {GetCurrentPlayerContext().DrawCardQuota}");
        return false;
    }

}

