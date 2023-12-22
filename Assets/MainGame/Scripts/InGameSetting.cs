using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new In-Game Setting",menuName = "In-Game Setting")]
public class InGameSetting : ScriptableObject
{
    public float turnDuration;
    public float endTurnDuration;
}
