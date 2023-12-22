
using UnityEngine;

public class MainGameManager : NetworkSingleton<MainGameManager>
{
    public delegate void GameState(InGameContext ctx);
    public static GameState OnGameStart;
    public static GameState OnStartTurn_1;
    public static GameState OnStartTurn_2;
    public static GameState OnEndTurn_1;
    public static GameState OnEndTurn_2;
    public static GameState OnGameEnd;

    [Header("Game Context")]
    public InGameContext inGameContext;
    protected override void InitAfterAwake()
    {
        InGameSetting inGameSetting = Resources.Load<InGameSetting>("Game Setting/In-Game Setting/SO/In-Game Setting");
        inGameContext = new(PlayerManager.Instance.playerData_1, PlayerManager.Instance.playerData_2, inGameSetting);
    }

}
