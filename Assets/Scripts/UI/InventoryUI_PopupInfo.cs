using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI_PopupInfo : MonoBehaviour
{
    [SerializeField] private RectTransform m_rectTransform;
    public RectTransform rectTransform => m_rectTransform;

    [SerializeField] private TextMeshProUGUI m_itemNameTxt;
    [SerializeField] private TextMeshProUGUI m_itemTypeTxt;
    [SerializeField] private TextMeshProUGUI m_rarityTxt;
    [SerializeField] private TextMeshProUGUI m_descriptionTxt;
    [SerializeField] private TextMeshProUGUI m_maxStackTxt;

    public void SetItemData(ItemData p_itemData) {
        m_itemNameTxt.text = p_itemData.itemName;
        Color rarityColor = GameManager.instance.GetRarityColor(p_itemData.rarity);
        m_itemNameTxt.color = rarityColor;
        m_itemTypeTxt.text = ItemData.GetItemTypeNames(p_itemData.itemType);
        m_rarityTxt.text = $"({p_itemData.rarity})";
        m_rarityTxt.color = rarityColor;
        m_descriptionTxt.text = p_itemData.itemDescription;
        m_maxStackTxt.text = $"max stack: {p_itemData.maxStackCount}";
    }


}
