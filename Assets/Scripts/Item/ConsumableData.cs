using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item_consumable_data", menuName = "Inventory Data/ItemData/Consumable", order = 0)]
public class ConsumableData : ItemData
{
    public enum ConsumableType {
        HealthPotion,
        MoneyBag,
        InventorySlotUpgrade
    }

    public override ItemType itemType => ItemType.Consumable;
    public ConsumableType consumableType;
    public int consumableValue = 0;

}
