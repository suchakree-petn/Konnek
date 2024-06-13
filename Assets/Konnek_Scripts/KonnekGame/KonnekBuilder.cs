using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class KonnekBuilder : NetworkBehaviour
{
    [Header("Animation Config")]
    [SerializeField] private float dropDuration;
    [SerializeField] private AnimationCurve dropCurve;

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
    }

    public void PlayPieceAnimation(Vector3 lastPosition,PlayPieceAnimation command)
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
            command.OnPlayPieceAnimationFinish?.Invoke();
        });
    }

    private void OnDisable()
    {
    }
}
