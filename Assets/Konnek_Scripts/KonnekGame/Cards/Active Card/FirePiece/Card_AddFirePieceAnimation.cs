using System.Collections.Generic;
using UnityEngine;

public class Card_AddFirePieceAnimation : Command
{
    uint instanceId;
    public Card_AddFirePieceAnimation(uint instanceId)
    {
        this.instanceId = instanceId;
    }

    public override void Execute()
    {
        CardManager cardManager = CardManager.Instance;
        Transform parent = cardManager.BoardParent;
        GameObject fireGO = GameObject.Instantiate(Card_FirePiece.FireVFX, parent);
        Vector3 position = KonnekManager.Instance.PlayedPositions[^1];
        fireGO.transform.localPosition = position;
        DebuffManager.DebuffGameObjectDict.Add(instanceId, fireGO);
        base.Execute();
    }

}