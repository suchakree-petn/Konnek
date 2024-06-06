
[System.Serializable]
public class PlayerContext
{
    public PlayerData PlayerData;
    public int PlayerOrderIndex;
    public bool IsPlayerTurn;
    public int DrawCardQuota;
    public int BonusDrawCardQuota;
    public bool IsPlayedPiece;
    public int PlayerMaxHp;
    public int PlayerCurrentHp;
    public int CardInHand;



    public PlayerContext(PlayerData playerData, int playerOrderIndex, int playerMaxHp)
    {
        PlayerData = playerData;
        PlayerOrderIndex = playerOrderIndex;
        IsPlayerTurn = playerOrderIndex == 1;
        DrawCardQuota = MainGameContext.DRAW_CARD_QUOTA_AMOUNT;
        BonusDrawCardQuota = 0;
        IsPlayedPiece = false;
        PlayerMaxHp = playerMaxHp;
        PlayerCurrentHp = playerMaxHp;
        CardInHand = 0;
    }

    public ulong GetClientId()
    {
        return PlayerData.ClientId;
    }
}
