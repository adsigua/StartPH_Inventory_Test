using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerData m_playerData;

    [SerializeField] private PlayerAnimationController m_playerAnim;

    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_runSpeed;

    [SerializeField] private float m_pickupRadius = 2.0f;
    [SerializeField] private float m_pickUpCoolDown = 0.3f;
    [SerializeField] private LayerMask m_pickableItemLayerMask;

    [SerializeField] private int m_maxInventorySlots = 40;

    private Vector2 m_moveInput;
    private Vector3 m_moveVector;
    private bool m_canPickup = true;

    private bool m_canMove = true;

    public static System.Action<PlayerData> OnOpenInventory;
    public static System.Action<int,bool> OnItemPickup;
    public static System.Action<int,int,Vector3> OnItemDrop;

    private void Start() {
        m_canPickup = true;
        m_playerData.LoadPlayerData();
    }

    private void Update() {
        if(Input.GetKey(KeyCode.I)) {
            m_canMove = false;
            OnOpenInventory?.Invoke(m_playerData);
        }
        if(m_canMove) {
            CheckPlayerMoveInput();
            CheckIfCanPickup();
        }
    }

    private void OnEnable() {
        InventoryUI.OnInventoryClose += OnInventoryClose;
        PlayerInventoryData.OnItemUse += OnItemUse;
        PlayerInventoryData.OnDropItems += OnDropItems;
    }

    private void OnDisable() {
        InventoryUI.OnInventoryClose -= OnInventoryClose;
        PlayerInventoryData.OnItemUse -= OnItemUse;
        PlayerInventoryData.OnDropItems -= OnDropItems;
    }

    private void OnItemUse(int p_itemId) {
        ConsumableData consumable = GameManager.instance.GetItemData(p_itemId) as ConsumableData;
        switch (consumable.consumableType) {
            case ConsumableData.ConsumableType.HealthPotion:
                m_playerData.UpdateHP(consumable.consumableValue);
                break;
            case ConsumableData.ConsumableType.MoneyBag:
                break;
            case ConsumableData.ConsumableType.InventorySlotUpgrade:
                break;
        }
    }

    private void OnDropItems(int p_itemId, int p_itemCount) {
        OnItemDrop?.Invoke(p_itemId, p_itemCount, transform.position);
    }

    private void OnInventoryClose() {
        m_canMove = true;
    }

    private void CheckPlayerMoveInput() {
        m_moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float speed = Input.GetAxisRaw("Run") > 0 ? m_runSpeed : m_moveSpeed;

        m_moveVector = new Vector3(m_moveInput.x, 0, m_moveInput.y).normalized * speed;

        if (m_moveInput.sqrMagnitude > 0.01f) {
            transform.LookAt(transform.position + m_moveVector.normalized, Vector3.up);
            transform.position += m_moveVector * Time.deltaTime;
        } else {
            speed = 0;
        }
        m_playerAnim.SetPickupLayer(speed > 0 ? 1 : 0);
        m_playerAnim.SetMoveAnim(speed);
    }

    private void CheckIfCanPickup() {
        if (Input.GetKeyDown(KeyCode.Space) && m_canPickup) {
            PickableItem item = GetPickableItem();
            if (item != null) {
                m_canPickup = false;
                m_playerAnim.DoPickupAnim();

                int inventoryIndex = m_playerData.inventoryData.GetInventorySlot(item.itemID);
                if(inventoryIndex > -1) {
                    item.PickupItem();
                    m_playerData.inventoryData.AddItem(item.itemID, inventoryIndex);
                    //m_playerData.SavePlayerData();
                }
                OnItemPickup?.Invoke(item.itemID, inventoryIndex>-1);
            }
            Invoke("OnPickUpCooldownEnd", m_pickUpCoolDown);
        }
    }

    private void OnPickUpCooldownEnd() {
        m_canPickup = true;
    }

    private PickableItem GetPickableItem() {
        Collider[] hits = Physics.OverlapSphere(transform.position, m_pickupRadius, m_pickableItemLayerMask);
        if(hits.Length > 0) {
            Collider closest = hits[0];
            return closest.GetComponent<PickableItem>();
        }
        return null;
    }

    [ContextMenu("save inventory")]
    public void SavePlayerData() {
        m_playerData.SavePlayerData();
    }

    [ContextMenu("load inventory")]
    public void LoadPlayerData() {
        m_playerData.LoadPlayerData();
    }


#if UNITY_EDITOR
    private void OnValidate() {
        if(m_playerData.inventoryData.inventoryItems.Count < m_maxInventorySlots) {
            m_playerData.inventoryData.UpdateInventorySlotsCount(m_maxInventorySlots);
        }
    }

    private void OnDrawGizmosSelected() {
        if (m_pickupRadius != 0) {
            Gizmos.DrawWireSphere(transform.position, m_pickupRadius);
        }
    }

#endif
}
