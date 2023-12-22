using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class KonnekBuilder : NetworkBehaviour
{
    private KonnekManager konnekManager => KonnekManager.Instance;
    [Header("Reference")]
    [SerializeField] private GameObject piece_prf;
    [SerializeField] private Transform board_parent;

    private void Awake()
    {
        if (board_parent == null)
        {
            board_parent = GameObject.FindWithTag("Board").transform;
        }
    }
    public void BuildBoard(InGameContext context)
    {
        List<Vector3> positions = context.playedPositions;
        Vector3 lastPosition = positions[^1];
        GameObject piece = Instantiate(piece_prf, board_parent);
        piece.transform.localPosition = new Vector3(lastPosition.x, lastPosition.y, 0);
        if (lastPosition.z == 1)
        {
            piece.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (lastPosition.z == 2)
        {
            piece.GetComponent<SpriteRenderer>().color = Color.blue;
        }else{
            Debug.LogError("Player Index Error");
        }
    }

    private void OnEnable()
    {
        KonnekManager.OnPlaySuccess += BuildBoard;
    }
    private void OnDisable()
    {
        KonnekManager.OnPlaySuccess -= BuildBoard;
    }
}
