using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SelectedColumnUI : NetworkSingleton<SelectedColumnUI>
{
    [SerializeField] private GameObject selected_prf;
    GameObject selected_ui;

    protected override void InitAfterAwake()
    {
        InitSelected_UI();
    }

    private void InitSelected_UI()
    {
        if (selected_ui == null)
        {
            selected_ui = Instantiate(selected_prf, transform);
            selected_ui.transform.localPosition = new(KonnekManager.Instance.SelectedColumn, 3.54f, 0);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            KonnekManager.Instance.selectedColumn.OnValueChanged += ShowSelectedColumn;
        }
    }

    private void ShowSelectedColumn(int previousValue, int newValue)
    {
        selected_ui.transform.localPosition = new(newValue, 3.54f, 0);

    }
}
