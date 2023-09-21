using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item_ingredient_data", menuName = "Inventory Data/ItemData/Crafting Item", order = 2)]
public class CraftItemData : ItemData
{
    public override ItemType itemType => ItemType.CraftItem;

}
