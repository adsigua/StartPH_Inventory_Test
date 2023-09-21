using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item_keyitem_data", menuName = "Inventory Data/ItemData/KeyItem", order = 3)]
public class KeyItemData : ItemData
{
    public override ItemType itemType => ItemType.KeyItem;
    public int useCount = 0;
    public int maxUseCount = 0;
}
