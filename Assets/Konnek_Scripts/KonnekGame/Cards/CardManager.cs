using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkSingleton<CardManager>
{

    private static uint cardInstanceId = 0;
    public static uint GetCardInstanceId()
    {
        uint currentId = cardInstanceId;
        cardInstanceId++;
        return currentId;
    }


    protected override void InitAfterAwake()
    {

    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            MainGameManager mainGameManager = MainGameManager.Instance;
            KonnekManager konnekManager = KonnekManager.Instance;

            // Fire piece trigger event
            // mainGameManager.OnStartTurn_Player_1 += HandleFirePieceProc;
            // mainGameManager.OnStartTurn_Player_2 += HandleFirePieceProc;
            // KonnekManager.OnPlayPieceSuccess += HandleCheckRemoveFirePiece;
        }
    }

    #region FirePiece

    [Header("Fire Piece")]
    public List<GameObject> FirePieceGO = new();
    public List<Vector3> FirePiece = new();
    public Transform BoardParent;

    public void AddFirePiece(GameObject firePieceGO)
    {
        Vector3 position = KonnekManager.Instance.PlayedPositions[^1];
        FirePieceGO.Add(firePieceGO);
        FirePiece.Add(position);
    }

    public void RemoveFirePiece(int index)
    {
        FirePieceGO.RemoveAt(index);
        FirePiece.RemoveAt(index);
    }

    // [ClientRpc]
    // public void AddCardFirePieceAnimation_ClientRpc(Vector3 position)
    // {
    //     Command command = new Card_FirePieceAnimation(position);
    //     MainGameManager.Instance.AddAnimationCommand(command);
    // }

    // public void HandleFirePieceProc(MainGameContext ctx)
    // {
    //     MainGameContext mainGameContext = MainGameManager.Instance.MainGameContext;
    //     ulong opponentClientId = mainGameContext.GetOpponentPlayerContext().GetClientId();
    //     Debug.Log("Opponent " + opponentClientId);
    //     ProcFirePiece_ClientRpc(opponentClientId);
    // }

    // [ClientRpc]
    // public void ProcFirePiece_ClientRpc(ulong opponentClientId)
    // {
    //     ulong clientId = NetworkManager.LocalClientId;
    //     MainGameContext mainGameContext = MainGameManager.Instance.MainGameContext;
    //     if (!mainGameContext.IsOwnerTurn(clientId)) return;

    //     int finalDamage = Damage * FirePiece.Count;
    //     if (finalDamage <= 0) return;
    //     KonnekManager.Instance.HpDecrease_ServerRpc(opponentClientId, finalDamage);
    // }

    private void HandleCheckRemoveFirePiece(MainGameContext mainGameContext)
    {
        KonnekManager konnekManager = KonnekManager.Instance;

        Vector3 lastPiecePos = konnekManager.PlayedPositions[^1];
        Vector3 pieceBelow = new(lastPiecePos.x, lastPiecePos.y - 1, lastPiecePos.z);
        int playerOrderIndex = (int)lastPiecePos.z;
        ulong clientId = mainGameContext.PlayerContextByPlayerOrder[playerOrderIndex].GetClientId();
        RemoveFirePieceEffect_ClientRpc(clientId, pieceBelow);
    }

    [ClientRpc]
    public void RemoveFirePieceEffect_ClientRpc(ulong clientId, Vector3 position)
    {
        if (NetworkManager.LocalClientId != clientId) return;

        RemoveFirePieceEffect(position);
    }

    public void RemoveFirePieceEffect(Vector3 position)
    {
        if (FirePiece.Contains(position))
        {
            int index = FirePiece.IndexOf(position);
            RemoveFirePiece(index);
        }
        else
        {
            Debug.LogWarning("Not contain this position");
        }
    }

    #endregion
}
