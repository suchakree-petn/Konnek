using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayOnClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        Command playCommand = new PlayPieceCommand((int)transform.localPosition.x,
            MainGameManager.Instance.MainGameContext.GetCurrentPlayerContext().PlayerOrderIndex);
        playCommand.OnComplete(TestCallback);
        MainGameManager.Instance.CommandQueue.AddCommand(playCommand);
    }
    private void TestCallback(){
            Debug.Log("Finish play at animation");

    }
}
