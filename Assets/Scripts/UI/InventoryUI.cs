using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, InventoryUI_PopupMenu.IPopupMenuListener {
    [SerializeField] private RectTransform m_rectTransform;
    [SerializeField] private List<InventoryUI_Item> m_uiItems = new List<InventoryUI_Item>();
    [SerializeField] private InventoryUI_Item m_weaponSlotItem;
    [SerializeField] private InventoryUI_Item m_armorSlotItem;
    [SerializeField] private InventoryUI_Item m_grabItem;
    [SerializeField] private InventoryUI_PopupInfo m_popupInfo;
    [SerializeField] private InventoryUI_PopupMenu m_popupMenu;

    [SerializeField] private TextMeshProUGUI m_playerHPTxt;
    [SerializeField] private TextMeshProUGUI m_playerDamageTxt;

    [SerializeField] private Button m_closeButton;
    [SerializeField] private Button m_saveButton;
    [SerializeField] private Button m_loadButton;
    [SerializeField] private Button m_clearButton;

    [SerializeField] private Button m_sortByType;
    [SerializeField] private Button m_sortByRarity;
    [SerializeField] private Button m_sortByCount;

    private PlayerData m_playerData;
    private InventoryUI_Item m_currentItem;

    public static System.Action OnInventoryClose;

  

    private void OnEnable() {
        m_closeButton.onClick.AddListener(() => ClosePlayerInventory());
        m_saveButton.onClick.AddListener(() => SavePlayerData());
        m_loadButton.onClick.AddListener(() => LoadPlayerData());
        m_clearButton.onClick.AddListener(() => ClearInventoryData());

        m_sortByType.onClick.AddListener(() => SortItems(PlayerInventoryData.SortType.Type));
        m_sortByRarity.onClick.AddListener(() => SortItems(PlayerInventoryData.SortType.Rarity));
        m_sortByCount.onClick.AddListener(() => SortItems(PlayerInventoryData.SortType.Count));

        m_popupMenu.RegisterListener(this);

        foreach (InventoryUI_Item uiitem in m_uiItems) {
            RegisterUIItemEvents(uiitem);
        }
        RegisterUIItemEvents(m_weaponSlotItem);
        RegisterUIItemEvents(m_armorSlotItem);
    }

    private void OnDisable() {
        m_closeButton.onClick.RemoveAllListeners();
        m_saveButton.onClick.RemoveAllListeners();
        m_loadButton.onClick.RemoveAllListeners();
        m_clearButton.onClick.RemoveAllListeners();

        m_sortByType.onClick.AddListener(() => SortItems(PlayerInventoryData.SortType.Type));
        m_sortByRarity.onClick.AddListener(() => SortItems(PlayerInventoryData.SortType.Rarity));
        m_sortByCount.onClick.AddListener(() => SortItems(PlayerInventoryData.SortType.Count));

        m_popupMenu.ClearEventListeners();
        foreach (InventoryUI_Item uiitem in m_uiItems) {
            UnregisterUIItemEvents(uiitem);
        }
        UnregisterUIItemEvents(m_weaponSlotItem);
        UnregisterUIItemEvents(m_armorSlotItem);
    }

    private void SortItems(PlayerInventoryData.SortType p_sortType) {
        m_playerData.inventoryData.SortItems(p_sortType);
        RefreshUI();
    }

    public void OpenPlayerInventory(PlayerData p_playerData) {
        m_playerData = p_playerData;
        RefreshUI();
    }

    private void RefreshUI() {
        m_popupMenu.gameObject.SetActive(false);
        m_grabItem.gameObject.SetActive(false);

        for (int i = 0; i < m_playerData.inventoryData.inventoryItems.Count; i++) {
            m_uiItems[i].SetInventoryData(m_playerData.inventoryData.inventoryItems[i]);
        }
        for (int i = m_playerData.inventoryData.inventoryItems.Count; i < m_uiItems.Count; i++) {
            m_uiItems[i].SetInventoryData(new PlayerInventoryData.InventoryItem());
        }

        m_weaponSlotItem.SetInventoryData(m_playerData.inventoryData.weaponSlot);
        m_armorSlotItem.SetInventoryData(m_playerData.inventoryData.armorSlot);

        int addedHp = m_playerData.addedHP;
        int addedDamage = m_playerData.addedDamage;

        string addedHpTxt = Mathf.Abs(addedHp) > 0 ? $" ({(addedHp >= 0 ? "+" : "-")}{Mathf.Abs(addedHp)})" : "";
        string addedDmgTxt = Mathf.Abs(addedDamage) > 0 ? $" ({(addedDamage >= 0 ? "+" : "-")}{Mathf.Abs(addedDamage)})" : "";

        m_playerHPTxt.text = $"HP : {m_playerData.currentHp} / {m_playerData.currentMaxHp + addedHp}{addedHpTxt}";
        m_playerDamageTxt.text = $"Damage : {m_playerData.baseDamage + addedDamage}{addedDmgTxt}";
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ClosePlayerInventory();
        }
    }

    public void ClosePlayerInventory() {
        m_popupMenu.gameObject.SetActive(false);
        m_popupInfo.gameObject.SetActive(false);
        m_grabItem.gameObject.SetActive(false);
        OnInventoryClose?.Invoke();
    }

    #region event registrations
    private void RegisterUIItemEvents(InventoryUI_Item p_uiitem) {
        p_uiitem.OnItemHover += OnItemHover;
        p_uiitem.OnItemHoverExit += OnItemHoverExit;
        p_uiitem.OnItemClick += OnItemClick;
        p_uiitem.OnItemRightClick += OnItemRightClick;
        p_uiitem.OnItemDrag += OnItemDrag;
        p_uiitem.OnItemDragEnd += OnItemDragEnd;
    }

    private void UnregisterUIItemEvents(InventoryUI_Item p_uiitem) {
        p_uiitem.OnItemHover -= OnItemHover;
        p_uiitem.OnItemHoverExit -= OnItemHoverExit;
        p_uiitem.OnItemClick -= OnItemClick;
        p_uiitem.OnItemRightClick -= OnItemRightClick;
        p_uiitem.OnItemDrag -= OnItemDrag;
        p_uiitem.OnItemDragEnd -= OnItemDragEnd;
    }
    #endregion

    private Vector2 GetRectAnchoredPos(Vector2 p_ScreenPos) {
        return new Vector2(
            p_ScreenPos.x / Screen.width * m_rectTransform.rect.width,
            p_ScreenPos.y / Screen.height * m_rectTransform.rect.height);
    }

    private void OnItemHover(InventoryUI_Item p_item) {
        if (m_grabItem.gameObject.activeInHierarchy) {
            return;
        }

        if (m_popupMenu.gameObject.activeInHierarchy) {
            if (m_currentItem == p_item) {
                return;
            } else {
                m_popupMenu.gameObject.SetActive(false);
            }
        }

        m_currentItem = p_item;
        m_popupInfo.SetItemData(GameManager.instance.GetItemData(p_item.itemId));

        m_popupInfo.rectTransform.anchoredPosition = GetRectAnchoredPos(Input.mousePosition);
        m_popupInfo.gameObject.SetActive(true);
    }

    private void OnItemHoverExit(InventoryUI_Item p_item) {
        m_popupInfo.gameObject.SetActive(false);
    }

    private void OnItemDragEnd(InventoryUI_Item p_dragItem, Vector2 p_screenPos) {
        m_grabItem.gameObject.SetActive(false);
        InventoryUI_Item destItem = GetInventoryUIByWorldPos(m_grabItem.transform.position);

        bool dragItemIsEquipped = p_dragItem == m_weaponSlotItem || p_dragItem == m_armorSlotItem;
        bool dragTargetIsEquipped = destItem == m_weaponSlotItem || destItem == m_armorSlotItem;

        if (destItem != null && destItem != p_dragItem) {
            if (dragItemIsEquipped && !dragTargetIsEquipped) {
                m_playerData.inventoryData.DragItemEquip(m_uiItems.IndexOf(destItem), 
                    p_dragItem == m_weaponSlotItem ? EquimentData.EquipmentType.Weapon : EquimentData.EquipmentType.Armor, false);
            } else if (!dragItemIsEquipped && dragTargetIsEquipped) {
                m_playerData.inventoryData.DragItemEquip(m_uiItems.IndexOf(p_dragItem),
                    destItem == m_weaponSlotItem ? EquimentData.EquipmentType.Weapon : EquimentData.EquipmentType.Armor, true);
            } else if(!dragItemIsEquipped){
                m_playerData.inventoryData.DragItemSlot(m_uiItems.IndexOf(p_dragItem), m_uiItems.IndexOf(destItem));
            }
            RefreshUI();
        }
    }

    private void OnItemDrag(InventoryUI_Item p_item, Vector2 p_screenPos) {
        if (!m_grabItem.gameObject.activeInHierarchy) {
            m_currentItem = p_item;
            m_grabItem.SetInventoryData(new PlayerInventoryData.InventoryItem() { itemId = p_item.itemId, itemCount = 0 });
            m_grabItem.gameObject.SetActive(true);
        }
        m_grabItem.rectTransform.anchoredPosition = GetRectAnchoredPos(p_screenPos);
    }

    private void OnItemClick(InventoryUI_Item item) {
        if (m_popupMenu.gameObject.activeInHierarchy) {
            m_popupMenu.gameObject.SetActive(false);
        }
    }

    private void OnItemRightClick(InventoryUI_Item p_item) {
        m_currentItem = p_item;
        m_popupMenu.rectTransform.anchoredPosition = GetRectAnchoredPos(Input.mousePosition);
        bool isEquipped = p_item == m_armorSlotItem || p_item == m_weaponSlotItem;

        if (isEquipped) {
            //EquimentData equipData = GameManager.instance.GetItemData(p_item.itemId) as EquimentData;
            //InventoryUI_Item equipSlot = equipData.equipmentType == EquimentData.EquipmentType.Weapon ? m_weaponSlotItem : m_armorSlotItem;
            PlayerInventoryData.InventoryItem invItem = p_item == m_weaponSlotItem ? m_playerData.inventoryData.weaponSlot : m_playerData.inventoryData.armorSlot;
            m_popupMenu.InitPopupMenu(invItem, true);
        } else {
            m_popupMenu.InitPopupMenu(m_playerData.inventoryData.inventoryItems[m_uiItems.IndexOf(p_item)], false);
        }
        m_popupMenu.gameObject.SetActive(true);
    }

    private InventoryUI_Item GetInventoryUIByWorldPos(Vector2 p_wPos) {
        InventoryUI_Item closestItem = null;
        float nearestSqrDist = 9999999;

        float halfWidth = m_uiItems[0].rectTransform.rect.width * 0.5f;
        float sqrWidth = halfWidth * halfWidth;
        float minSqrDist = sqrWidth * (1.8f * 1.8f);

        List<InventoryUI_Item> testItems = new List<InventoryUI_Item>(m_uiItems);
        testItems.Insert(0, m_armorSlotItem);
        testItems.Insert(0, m_weaponSlotItem);

        foreach (InventoryUI_Item uiItem in testItems) {
            Bounds bound = RectTransformUtility.CalculateRelativeRectTransformBounds(m_grabItem.rectTransform, uiItem.rectTransform);
            float sqrDist = bound.center.sqrMagnitude;
            if (sqrDist < nearestSqrDist && sqrDist <= minSqrDist) {
                nearestSqrDist = sqrDist;
                closestItem = uiItem;
            }
            if (sqrDist <= sqrWidth) {
                return uiItem;
            }
        }

        return closestItem;
    }

    #region popupmenu 
    public void OnUseMenuClick() {
        m_playerData.inventoryData.UseItemSlot(m_uiItems.IndexOf(m_currentItem));
        RefreshUI();
    }

    public void OnEquipMenuClick() {
        m_playerData.inventoryData.EquipItem(m_uiItems.IndexOf(m_currentItem));
        m_playerData.UpdateHP(0);
        RefreshUI();
    }

    public void OnSplitMenuClick() {
        m_playerData.inventoryData.SplitItems(m_uiItems.IndexOf(m_currentItem));
        RefreshUI();
    }

    public void OnDropMenuClick() {
        if (m_currentItem == m_weaponSlotItem) {
            m_playerData.inventoryData.DropEquipItem(m_playerData.inventoryData.weaponSlot);
        } else if (m_currentItem == m_armorSlotItem) {
            m_playerData.inventoryData.DropEquipItem(m_playerData.inventoryData.armorSlot);
        } else { 
            m_playerData.inventoryData.DropItemSlot(m_uiItems.IndexOf(m_currentItem));
        }
        RefreshUI();
    }

    public void OnDestroyMenuClick() {
        m_playerData.inventoryData.DestroyItemSlot(m_uiItems.IndexOf(m_currentItem));
        RefreshUI();
    }

    public void OnUnequipClick() {
        if(m_currentItem == m_weaponSlotItem) {
            m_playerData.inventoryData.TryUnequip(ref m_playerData.inventoryData.weaponSlot);
        } else {
            m_playerData.inventoryData.TryUnequip(ref m_playerData.inventoryData.armorSlot);
        }
        RefreshUI();
    }

    #endregion


    private void SavePlayerData() {
        m_playerData.SavePlayerData();
    }

    private void LoadPlayerData() {
        m_playerData.LoadPlayerData();
        RefreshUI();
    }

    private void ClearInventoryData() {
        m_playerData.ClearPlayerData();
        RefreshUI();
    }

}
