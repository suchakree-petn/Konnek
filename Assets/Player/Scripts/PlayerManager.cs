using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkSingleton<PlayerManager>
{
    public PlayerData playerData_1;
    public PlayerData playerData_2;
    protected override void InitAfterAwake()
    {
    }

    
}
