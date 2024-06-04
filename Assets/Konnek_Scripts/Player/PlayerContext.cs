
[System.Serializable]
public class PlayerContext
{
    public PlayerData PlayerData;
    public int PlayerOrderIndex;
    public bool IsPlayerTurn;
    public int DrawCardQuota;
    public bool IsPlayedPiece;
    public int PlayerHp;
    public PlayerContext(PlayerData playerData, int playerOrderIndex,int playerHp)
    {
        PlayerData = playerData;
        PlayerOrderIndex = playerOrderIndex;
        IsPlayerTurn = playerOrderIndex == 1;
        IsPlayedPiece = false;
        PlayerHp = playerHp;
    }

    public ulong GetClientId()
    {
        return PlayerData.ClientId;
    }
}
