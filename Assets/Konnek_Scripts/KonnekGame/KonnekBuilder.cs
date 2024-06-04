using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class KonnekBuilder : NetworkBehaviour
{
    private KonnekManager KonnekManager => KonnekManager.Instance;

    [Header("Animation Config")]
    [SerializeField] private float dropDuration;
    [SerializeField] private AnimationCurve dropCurve;
    public static Action OnCompleteDropAnimation;

    [Header("Reference")]
    [SerializeField] private Transform piece_prf;
    private Transform board_parent;

    private void Awake()
    {
        if (board_parent == null)
        {
            board_parent = GameObject.FindWithTag("Board").transform;
        }
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }
        KonnekManager.OnPlayPieceSuccess += (context) => BuildBoardClientRpc(KonnekManager.KonnekBoard[^1]);
    }
    [ClientRpc]
    public void BuildBoardClientRpc(Vector3 lastPosition)
    {
        Transform piece = Instantiate(piece_prf,board_parent);
        piece.gameObject.SetActive(false);
        piece.localPosition = new Vector3(lastPosition.x, 7, 0);
        if (lastPosition.z == 1)
        {
            piece.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (lastPosition.z == 2)
        {
            piece.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            Debug.LogError("Player Index Error");
        }
        piece.gameObject.SetActive(true);
        piece.DOLocalMoveY(lastPosition.y, dropDuration).SetEase(dropCurve)
        .OnComplete(() =>
        {
            if (!IsServer) return;
            OnCompleteDropAnimation?.Invoke();
        });
    }

    private void OnDisable()
    {
    }
}
