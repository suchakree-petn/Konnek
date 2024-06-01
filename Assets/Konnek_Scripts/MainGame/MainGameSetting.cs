using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName ="new Main Game Setting",menuName = "Main Game Setting")]
public class MainGameSetting : ScriptableObject
{
    public float turnDuration = 10;
    public float endTurnDuration = 3;

}
