using UnityEngine;

public partial class MainGameManager
{
    public delegate void GameState(ref MainGameContext ctx);
    public static GameState OnGameStart;
    public static GameState OnStartTurn_Player_1;
    public static GameState OnDuringTurn_Player_1;
    public static GameState OnEndTurn_Player_1;
    public static GameState OnStartTurn_Player_2;
    public static GameState OnDuringTurn_Player_2;
    public static GameState OnEndTurn_Player_2;
    public static GameState OnGameEnd;
    private void GameStateRunner()
    {
        switch (mainGameContext.currentState)
        {
            case MainGameState.Player_1_Start_Turn:
                OnStartTurn_Player_1?.Invoke(ref mainGameContext);
                mainGameContext.currentState = MainGameState.Player_1_During_Turn;
                break;
            case MainGameState.Player_1_During_Turn:
                OnDuringTurn_Player_1?.Invoke(ref mainGameContext);
                break;
            case MainGameState.Player_1_End_Turn:
                OnEndTurn_Player_1?.Invoke(ref mainGameContext);
                Debug.Log("Player 1 end turn");
                mainGameContext.currentState = MainGameState.Player_2_Start_Turn;
                break;
            case MainGameState.Player_2_Start_Turn:
                OnStartTurn_Player_2?.Invoke(ref mainGameContext);
                mainGameContext.currentState = MainGameState.Player_2_During_Turn;
                break;
            case MainGameState.Player_2_During_Turn:
                OnDuringTurn_Player_2?.Invoke(ref mainGameContext);
                break;
            case MainGameState.Player_2_End_Turn:
                OnEndTurn_Player_2?.Invoke(ref mainGameContext);
                Debug.Log("Player 2 end turn");
                mainGameContext.currentState = MainGameState.Player_1_Start_Turn;
                break;
            default:
                break;
        }
    }
}
public enum MainGameState
{
    Player_1_Start_Turn,
    Player_1_During_Turn,
    Player_1_End_Turn,
    Player_2_Start_Turn,
    Player_2_During_Turn,
    Player_2_End_Turn,
    Idle,
    Default
}
