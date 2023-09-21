using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private InventoryUI m_uiInventory;
    //[SerializeField] private IngameUI m_ingameUI;

    private void OnEnable() {
        PlayerManager.OnOpenInventory += OnOpenInventory;
        InventoryUI.OnInventoryClose += OnInventoryClose;

    }

    private void OnDisable() {
        PlayerManager.OnOpenInventory -= OnOpenInventory;
        InventoryUI.OnInventoryClose -= OnInventoryClose;
    }


    private void OnOpenInventory(PlayerData p_playerData) {
        m_uiInventory.gameObject.SetActive(true);
        m_uiInventory.OpenPlayerInventory(p_playerData);
    }

    private void OnInventoryClose() {
        m_uiInventory.gameObject.SetActive(false);
    }

}
