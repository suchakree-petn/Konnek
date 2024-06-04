
[System.Serializable]
public class PlayerContext
{
    public PlayerData PlayerData;
    public int PlayerOrderIndex;
    public bool IsPlayerTurn;
    public int DrawCardQuota;
    public bool IsPlayedPiece;

    public PlayerContext(PlayerData playerData, int playerOrderIndex)
    {
        PlayerData = playerData;
        PlayerOrderIndex = playerOrderIndex;
        IsPlayerTurn = playerOrderIndex == 1;
        IsPlayedPiece = false;
    }

    public ulong GetClientId()
    {
        return PlayerData.ClientId;
    }
}
