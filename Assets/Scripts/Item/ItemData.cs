using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemData : ScriptableObject
{
    static string[] m_itemTypeNames = { "Consumable", "Equipment", "Crafting Item", "Key Item", "Trade Item" };
    public static string GetItemTypeNames(ItemType p_itemType) => m_itemTypeNames[(int)p_itemType];

    public enum RarityType {
        Common, Unique, Rare, Epic, Legendary
    }

    public enum ItemType {
        CraftItem,
        Equipment,
        Consumable,
        KeyItem,
        TradeItem
    }

    public virtual ItemType itemType => ItemType.Consumable;
    public int itemID = -1;
    public string itemName;
    public Sprite itemSprite;
    public Texture itemTexture;
    public string itemDescription;
    public RarityType rarity;
    public int maxStackCount;
    public int baseBuyPrice;
    public int baseSellPrice;
}
