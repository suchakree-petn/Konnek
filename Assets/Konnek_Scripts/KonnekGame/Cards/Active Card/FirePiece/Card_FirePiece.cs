using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_FirePiece", menuName = "Card/Action Card/Fire Piece")]
public class Card_FirePiece : Card
{
    public int Damage;
    public int Duration;
    private ulong clientId;

    private static GameObject fireVFX;
    public static GameObject FireVFX
    {
        get
        {
            if (fireVFX == null)
            {
                fireVFX = Resources.Load<GameObject>("Prefab/CardVFX/FirePieceVFX");
            }
            return fireVFX;
        }
    }


    public override void PlayCard(ulong clientId)
    {
        this.clientId = clientId;
        KonnekManager.OnPlayPieceSuccess -= Card_FirePiece_OnPlayPieceSuccess;
        KonnekManager.OnPlayPieceSuccess += Card_FirePiece_OnPlayPieceSuccess;

        OnFinishPlayCard?.Invoke();
    }

    private void Card_FirePiece_OnPlayPieceSuccess(MainGameContext context)
    {
        CardManager cardManager = CardManager.Instance;
        KonnekManager konnekManager = KonnekManager.Instance;
        Vector3 lastPiecePos = konnekManager.PlayedPositions[^1];
        DebuffManager.Instance.AddDebuff(DebuffType.Fire, Damage, clientId, Duration);

        // cardManager.AddCardFirePieceAnimation_ClientRpc(lastPiecePos);

        KonnekManager.OnPlayPieceSuccess -= Card_FirePiece_OnPlayPieceSuccess;
    }


}