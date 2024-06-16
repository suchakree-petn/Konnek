using UnityEngine;
using UnityEngine.Rendering;

public class Card_RemoveFirePieceAnimation : Command
{
    uint instanceId;
    public Card_RemoveFirePieceAnimation(uint instanceId)
    {
        this.instanceId = instanceId;
    }

    public override void Execute()
    {
        GameObject fireGO = DebuffManager.DebuffGameObjectDict[instanceId];
        GameObject.Destroy(fireGO);
        DebuffManager.DebuffGameObjectDict.Remove(instanceId);
        base.Execute();
    }

}