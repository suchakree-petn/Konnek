using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    [SerializeField] private GameObject square;
    [SerializeField] private Transform slots_parent;
    
    [ContextMenu("Generate Board")]
    private void Generate()
    {
        if (transform.childCount == 1)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        Transform slotParent = Instantiate(slots_parent, transform);
        for (int i = 1; i < 8; i++)
        {
            for (int j = 1; j < 7; j++)
            {
                GameObject slot = Instantiate(square, slotParent);
                slot.transform.localPosition = new Vector3(i, j, 0);
            }
        }
    }
}
