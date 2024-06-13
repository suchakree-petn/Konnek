using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class MainGameContext
{
    public const int PLAYER_MAX_HP = 50;
    public const int START_CARD_AMOUNT = 4;
    public const int DRAW_CARD_QUOTA_AMOUNT = 1;
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
            Dictionary<int, PlayerContext> result = new()
            {
                {playerContext_1.PlayerOrderIndex,playerContext_1},
                {playerContext_2.PlayerOrderIndex,playerContext_2}
            };
            return result;
        }
    }

    public float TurnDuration => mainGameSetting.turnDuration;
    public float currentTurnDuration;


    public MainGameContext(PlayerData player1, PlayerData player2, MainGameSetting mainGameSetting)
    {
        playerContext_1 = new(player1, 1, PLAYER_MAX_HP);
        playerContext_2 = new(player2, 2, PLAYER_MAX_HP);
        currentPlayer = playerContext_1;
        this.mainGameSetting = mainGameSetting;
        currentTurnDuration = mainGameSetting.turnDuration;
    }


    public PlayerContext GetPlayerContextByIndex(int playerIndex)
    {
        if (playerIndex == 1)
        {
            return playerContext_1;
        }
        return playerContext_2;
    }
    public PlayerContext GetPlayerContextByPlayerId(string playerId)
    {
        if (playerId == playerContext_1.PlayerData.PlayerId)
        {
            return playerContext_1;
        }
        return playerContext_2;
    }
    public PlayerContext GetPlayerContextByClientId(ulong clientId)
    {
        if (playerContext_1.GetClientId() == clientId)
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
    public PlayerContext GetOpponentPlayerContext()
    {
        if (currentPlayer.PlayerOrderIndex == 2)
        {
            return playerContext_1;
        }
        return playerContext_2;
    }
    public PlayerContext GetOwnerPlayerContext()
    {
        return GetPlayerContextByClientId(NetworkManager.Singleton.LocalClientId);
    }
    public void SetCurrentPlayerContext(int playerIndex)
    {
        PlayerContext currentPlayer = GetPlayerContextByIndex(playerIndex);
        this.currentPlayer = currentPlayer;
    }
    public bool IsOwnerTurn(ulong clientId)
    {
        return MainGameManager.Instance.CurrentClientTurn == clientId;
    }
    public int GetPlayerIndexByClientId(ulong clientId)
    {
        if (clientId == playerContext_1.GetClientId())
        {
            return playerContext_1.PlayerOrderIndex;
        }
        return playerContext_2.PlayerOrderIndex;

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

    public void SetPlayerHp(ulong clientId, int amount)
    {
        PlayerContext playerContext = PlayerContextByClientId[clientId];
        playerContext.PlayerCurrentHp = amount;
    }

    public int GetPlayerHp(ulong clientId)
    {
        return PlayerContextByClientId[clientId].PlayerCurrentHp;
    }
}

