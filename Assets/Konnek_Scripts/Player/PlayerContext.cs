
[System.Serializable]
public class PlayerContext
{
    public PlayerData PlayerData;
    public int PlayerOrderIndex;
    public bool IsPlayerTurn;
    public int DrawCardQuota;

    public PlayerContext(PlayerData playerData, int playerOrderIndex)
    {
        PlayerData = playerData;
        PlayerOrderIndex = playerOrderIndex;
        IsPlayerTurn = playerOrderIndex == 1;
    }

    public ulong GetClientId()
    {
        return PlayerData.ClientId;
    }
}
