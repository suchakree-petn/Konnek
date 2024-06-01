using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayOnClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        Command playCommand = new PlayAtCommand((int)transform.localPosition.x,
            MainGameManager.Instance.mainGameContext.GetCurrentPlayerContext().playerOrderIndex);
        playCommand.OnComplete(TestCallback);
        MainGameManager.Instance.commandQueue.AddCommand(playCommand);
    }
    private void TestCallback(){
            Debug.Log("Finish play at animation");

    }
}
