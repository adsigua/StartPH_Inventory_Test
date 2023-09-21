using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InventoryUI_PopupMenu : MonoBehaviour
{
    public interface IPopupMenuListener {
        public void OnUseMenuClick();
        public void OnEquipMenuClick();
        public void OnSplitMenuClick();
        public void OnDropMenuClick();
        public void OnDestroyMenuClick();
        public void OnUnequipClick();
    }

    [SerializeField] private RectTransform m_rectTransform;
    public RectTransform rectTransform => m_rectTransform;

    public enum PopupMenuAction {
        Use,
        Equip,
        Split,
        Drop,
        Destroy,
        Unequip,
    }

    [SerializeField] private List<Button> m_popupMenuButtons;

    public Button this[PopupMenuAction p_action] => m_popupMenuButtons[(int)p_action];

    public void InitPopupMenu(PlayerInventoryData.InventoryItem p_inventoryItem, bool p_isEquipped) {
        ItemData itemData = GameManager.instance.GetItemData(p_inventoryItem.itemId);

        bool isConsumable = itemData.itemType == ItemData.ItemType.Consumable;
        bool isEquipable = itemData.itemType == ItemData.ItemType.Equipment;
        bool canBeSplit = p_inventoryItem.itemCount > 1;

        this[PopupMenuAction.Use].gameObject.SetActive(isConsumable);
        this[PopupMenuAction.Equip].gameObject.SetActive(isEquipable && !p_isEquipped);
        this[PopupMenuAction.Unequip].gameObject.SetActive(isEquipable && p_isEquipped);
        this[PopupMenuAction.Split].gameObject.SetActive(canBeSplit);
    }

    public void RegisterListener(IPopupMenuListener p_listener) {
        this[PopupMenuAction.Use].onClick.AddListener(() => p_listener.OnUseMenuClick());
        this[PopupMenuAction.Equip].onClick.AddListener(() => p_listener.OnEquipMenuClick());
        this[PopupMenuAction.Split].onClick.AddListener(() => p_listener.OnSplitMenuClick());
        this[PopupMenuAction.Drop].onClick.AddListener(() => p_listener.OnDropMenuClick());
        this[PopupMenuAction.Destroy].onClick.AddListener(() => p_listener.OnDestroyMenuClick());
        this[PopupMenuAction.Unequip].onClick.AddListener(() => p_listener.OnUnequipClick());
    }

    public void ClearEventListeners() {
        foreach(Button btn in m_popupMenuButtons) {
            btn.onClick.RemoveAllListeners();
        }
    }
}
