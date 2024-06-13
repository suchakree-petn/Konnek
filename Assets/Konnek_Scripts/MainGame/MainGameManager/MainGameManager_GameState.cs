using UnityEngine;

public partial class MainGameManager
{
    public delegate void GameState(MainGameContext ctx);
    public GameState OnStartGame;
    public GameState OnStartTurn_Player_1;
    public GameState OnDuringTurn_Player_1;
    public GameState OnEndTurn_Player_1;
    public GameState OnStartTurn_Player_2;
    public GameState OnDuringTurn_Player_2;
    public GameState OnEndTurn_Player_2;
    public GameState OnGameEnd;

    private bool isStartGame;
    
    private void GameStateRunner()
    {
        switch (GetCurrentGameState())
        {
            case MainGameState.StartGame:
                if (!isStartGame)
                {
                    OnStartGame?.Invoke(MainGameContext);
                    isStartGame = true;
                }
                if (AnimationQueue.commandsQueue.Count == 0)
                {
                    SetCurrentGameState(MainGameState.Player_1_Start_Turn);
                }
                break;
            case MainGameState.Player_1_Start_Turn:
                OnStartTurn_Player_1?.Invoke(MainGameContext);
                SetCurrentGameState(MainGameState.Player_1_During_Turn);
                break;
            case MainGameState.Player_1_During_Turn:
                OnDuringTurn_Player_1?.Invoke(MainGameContext);
                break;
            case MainGameState.Player_1_End_Turn:
                OnEndTurn_Player_1?.Invoke(MainGameContext);
                Debug.Log("Player 1 end turn");
                SetCurrentGameState(MainGameState.Player_2_Start_Turn);
                break;
            case MainGameState.Player_2_Start_Turn:
                OnStartTurn_Player_2?.Invoke(MainGameContext);
                SetCurrentGameState(MainGameState.Player_2_During_Turn);
                break;
            case MainGameState.Player_2_During_Turn:
                OnDuringTurn_Player_2?.Invoke(MainGameContext);
                break;
            case MainGameState.Player_2_End_Turn:
                OnEndTurn_Player_2?.Invoke(MainGameContext);
                Debug.Log("Player 2 end turn");
                SetCurrentGameState(MainGameState.Player_1_Start_Turn);
                break;
            default:
                break;
        }
    }
}
public enum MainGameState
{
    StartGame,
    Player_1_Start_Turn,
    Player_1_During_Turn,
    Player_1_End_Turn,
    Player_2_Start_Turn,
    Player_2_During_Turn,
    Player_2_End_Turn,
    Idle,
    EndGame,
    Default
}
