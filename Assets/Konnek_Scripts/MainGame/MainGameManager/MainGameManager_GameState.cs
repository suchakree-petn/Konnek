using UnityEngine;

public partial class MainGameManager
{
    public delegate void GameState(MainGameContext ctx);
    public GameState OnGameStart;
    public GameState OnStartTurn_Player_1;
    public GameState OnDuringTurn_Player_1;
    public GameState OnEndTurn_Player_1;
    public GameState OnStartTurn_Player_2;
    public GameState OnDuringTurn_Player_2;
    public GameState OnEndTurn_Player_2;
    public GameState OnGameEnd;
    private void GameStateRunner()
    {
        switch (MainGameContext.currentState)
        {
            case MainGameState.Player_1_Start_Turn:
                OnStartTurn_Player_1?.Invoke(MainGameContext);
                MainGameContext.currentState = MainGameState.Player_1_During_Turn;
                break;
            case MainGameState.Player_1_During_Turn:
                OnDuringTurn_Player_1?.Invoke(MainGameContext);
                break;
            case MainGameState.Player_1_End_Turn:
                OnEndTurn_Player_1?.Invoke(MainGameContext);
                Debug.Log("Player 1 end turn");
                MainGameContext.currentState = MainGameState.Player_2_Start_Turn;
                break;
            case MainGameState.Player_2_Start_Turn:
                OnStartTurn_Player_2?.Invoke(MainGameContext);
                MainGameContext.currentState = MainGameState.Player_2_During_Turn;
                break;
            case MainGameState.Player_2_During_Turn:
                OnDuringTurn_Player_2?.Invoke(MainGameContext);
                break;
            case MainGameState.Player_2_End_Turn:
                OnEndTurn_Player_2?.Invoke(MainGameContext);
                Debug.Log("Player 2 end turn");
                MainGameContext.currentState = MainGameState.Player_1_Start_Turn;
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
