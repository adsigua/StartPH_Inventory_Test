using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class InventoryUI_Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform m_rectTransform;
    [SerializeField] private RectTransform m_iconRectTransform;
    [SerializeField] private TextMeshProUGUI m_itemCount;
    [SerializeField] private Image m_iconImage;
    [SerializeField] private PlayerInventoryData.InventoryItem m_inventoryData;
    public int itemId => m_inventoryData.itemId;

    public RectTransform rectTransform => m_rectTransform;
    public Action<InventoryUI_Item> OnItemHover;
    public Action<InventoryUI_Item> OnItemHoverExit;
    public Action<InventoryUI_Item> OnItemClick;
    public Action<InventoryUI_Item> OnItemRightClick;
    public Action<InventoryUI_Item, Vector2> OnItemDrag;
    public Action<InventoryUI_Item, Vector2> OnItemDragEnd;

    public void OnPointerEnter(PointerEventData eventData) {
        if (m_inventoryData.itemId != -1) {
            OnItemHover?.Invoke(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (m_inventoryData.itemId != -1) {
            OnItemHoverExit?.Invoke(this);
        }
    }

    public void SetInventoryData(PlayerInventoryData.InventoryItem p_itemData) {
        m_inventoryData = p_itemData;
        if (p_itemData.itemId != -1) {
            m_iconRectTransform.gameObject.SetActive(true);
            m_iconImage.sprite = GameManager.instance.GetItemData(p_itemData.itemId).itemSprite;
            m_itemCount.text = p_itemData.itemCount.ToString();
        } else {
            m_iconRectTransform.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData p_eventData) {

        if(p_eventData.button == PointerEventData.InputButton.Left) {
            OnItemClick?.Invoke(this);
        } else if (p_eventData.button == PointerEventData.InputButton.Right && m_inventoryData.itemId != -1) {
            OnItemRightClick?.Invoke(this);
        }
    }

    public void OnDrag(PointerEventData p_eventData) {
        if(p_eventData.button == PointerEventData.InputButton.Left && m_inventoryData.itemId != -1) {
            OnItemDrag?.Invoke(this, p_eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData p_eventData) {
        OnItemDragEnd?.Invoke(this, p_eventData.position);
    }
}
