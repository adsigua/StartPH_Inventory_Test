using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerInventoryData {
    [System.Serializable]
    public class InventoryItem {
        public int itemId = -1;
        public int itemCount = 0;
    }

    public enum SortType {
        Type,
        Rarity,
        Count,
    }

    public PlayerInventoryData(int p_slotCount = 48) {
        inventoryItems = new List<InventoryItem>();
        for(int i=0; i<p_slotCount; i++) {
            inventoryItems.Add(new InventoryItem());
        }
        weaponSlot = new InventoryItem();
        armorSlot = new InventoryItem();
    }

    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public List<int> hotBarItems;

    public InventoryItem weaponSlot = new InventoryItem();
    public InventoryItem armorSlot = new InventoryItem();

    public static System.Action<int,int> OnDropItems;
    public static System.Action<int> OnItemUse;

    public int GetInventorySlot(int p_itemId) {
        List<int> vacantSpaces = new List<int>();
        //check if existing and if can stack
        for (int i = 0; i < inventoryItems.Count; i++) {
            //check if same id and if current count is less that maxstack
            if (inventoryItems[i].itemId == p_itemId && inventoryItems[i].itemCount < GameManager.instance.GetItemData(p_itemId).maxStackCount) {
                return i;
            } else if (inventoryItems[i].itemId == -1) {     //if not same id check if space is vacant, save for later
                vacantSpaces.Add(i);
            }
        }
        //Check if vacant space available
        if (vacantSpaces.Count > 0) {
            return vacantSpaces[0];
        }
        return -1;
    }

    public void AddItem(int p_itemID, int p_inventoryIndex) {
        if(inventoryItems[p_inventoryIndex].itemId == -1) {
            inventoryItems[p_inventoryIndex].itemId = p_itemID;
            inventoryItems[p_inventoryIndex].itemCount = 1;
        } else {
            inventoryItems[p_inventoryIndex].itemCount++;
        }
    }

    public List<InventoryItem> UpdateInventorySlotsCount(int p_newSlotCount) {
        //add item slots based on p_newSlotCount
        if(inventoryItems.Count < p_newSlotCount) {
            while(inventoryItems.Count != p_newSlotCount) { 
                InventoryItem item = new InventoryItem { itemId = -1, itemCount = 0 };
                inventoryItems.Add(item);
            }
        } 
        //remove slots and throw items that are in the slots
        else if(inventoryItems.Count > p_newSlotCount){      
            List<InventoryItem> toThrow = new List<InventoryItem>();
            while(inventoryItems.Count > p_newSlotCount) { 
                InventoryItem item = inventoryItems[inventoryItems.Count - 1];
                inventoryItems.Remove(item);
                if(item.itemId != -1) { 
                    toThrow.Add(item);
                }
            }
            return toThrow;
        }
        return new List<InventoryItem>();

    }

    public void EquipItem(int p_inventoryIndex) {
        InventoryItem invItem = inventoryItems[p_inventoryIndex];
        EquimentData equipData = GameManager.instance.GetItemData(invItem.itemId) as EquimentData;

        ref InventoryItem targetSlot = ref weaponSlot;
        if(equipData.equipmentType == EquimentData.EquipmentType.Armor) {
            targetSlot = ref armorSlot;
        }

        if(targetSlot.itemId != -1) {
            SwapEquipSlot(p_inventoryIndex, ref targetSlot);
        } else {
            targetSlot = new InventoryItem() { itemId = invItem.itemId, itemCount = invItem.itemCount };
            DestroyItemSlot(p_inventoryIndex);
        }
    }

    public void TryUnequip(ref InventoryItem p_inventoryItem) {
        int inventoryIndex = -1;
        for (int i = 0; i < inventoryItems.Count; i++) {
            if (inventoryItems[i].itemId == -1) {
                inventoryIndex = i;
                break;
            }
        }

        //no vacant to unequip
        if(inventoryIndex == -1) {
            return;
        }

        inventoryItems[inventoryIndex] = new InventoryItem() { itemId = p_inventoryItem.itemId, itemCount = p_inventoryItem.itemCount };
        p_inventoryItem = new InventoryItem();
    }

    private void SwapEquipSlot(int p_invIndex,ref InventoryItem p_targetSlot) {
        InventoryItem temp = inventoryItems[p_invIndex];
        inventoryItems[p_invIndex] = new InventoryItem() { itemId = p_targetSlot.itemId, itemCount = p_targetSlot.itemCount };
        p_targetSlot = temp;
    }

    public void DragItemEquip(int p_inventoryIndex, EquimentData.EquipmentType p_equipType, bool p_targetIsEquipSlot) {
        InventoryItem invItem = inventoryItems[p_inventoryIndex];
        InventoryItem slotItem = p_equipType == EquimentData.EquipmentType.Weapon ? weaponSlot : armorSlot;

        InventoryItem targetSlot;
        if (p_targetIsEquipSlot) {
            targetSlot = slotItem;
            ItemData fromData = GameManager.instance.GetItemData(invItem.itemId);
            //if towards equip slot but null, not equipment, or wrong type
            if (fromData == null || fromData.itemType != ItemData.ItemType.Equipment || (fromData.itemType == ItemData.ItemType.Equipment && ((EquimentData)fromData).equipmentType != p_equipType)) {
                return;
            }

        } else {
            targetSlot = invItem;
        }

        ItemData targetItemData = GameManager.instance.GetItemData(targetSlot.itemId);
        bool vacantSpace = targetSlot.itemId == -1;
        bool sametype = false;

       
        

        if (targetSlot.itemId != -1 && targetItemData.itemType == ItemData.ItemType.Equipment) {
            EquimentData equipData = targetItemData as EquimentData;
            sametype = equipData.equipmentType == p_equipType;
        }
        if (vacantSpace || sametype) {
            InventoryItem temp = inventoryItems[p_inventoryIndex];
            if(p_equipType == EquimentData.EquipmentType.Weapon) {
                inventoryItems[p_inventoryIndex] = new InventoryItem() { itemId = weaponSlot.itemId, itemCount = weaponSlot.itemCount };
                weaponSlot = new InventoryItem() { itemId = temp.itemId, itemCount = temp.itemCount };
            } else {
                inventoryItems[p_inventoryIndex] = new InventoryItem() { itemId = armorSlot.itemId, itemCount = armorSlot.itemCount };
                armorSlot = new InventoryItem() { itemId = temp.itemId, itemCount = temp.itemCount };
            }
        }
    }

    public void DragItemSlot(int p_fromIndex, int p_toIndex) {
        bool sameItemId = inventoryItems[p_fromIndex].itemId == inventoryItems[p_toIndex].itemId;
        bool destIsMaxStack = false;
        if(inventoryItems[p_toIndex].itemId != -1) {
            destIsMaxStack = inventoryItems[p_toIndex].itemCount >= GameManager.instance.GetItemData(inventoryItems[p_toIndex].itemId).maxStackCount;
        }
        if(sameItemId && !destIsMaxStack) {
            StackItems(p_fromIndex, p_toIndex);
        } else if(!sameItemId || destIsMaxStack) {
            SwapItemSlots(p_fromIndex, p_toIndex);
        }
    }

    public void UseItemSlot(int p_index) {
        InventoryItem item = inventoryItems[p_index];
        OnItemUse?.Invoke(item.itemId);
        item.itemCount--;

        if(item.itemCount <= 0) {
            DestroyItemSlot(p_index);
        }
    }

    public void StackItems(int p_fromIndex, int p_toIndex) {
        InventoryItem from = inventoryItems[p_fromIndex];
        InventoryItem to = inventoryItems[p_toIndex];
        int toMaxStack = GameManager.instance.GetItemData(to.itemId).maxStackCount;
        int availableSpace = toMaxStack - to.itemCount;
        int countToMove = Mathf.Min(availableSpace, from.itemCount);

        to.itemCount += countToMove;
        from.itemCount -= countToMove;

        if(from.itemCount <= 0) {
            DestroyItemSlot(p_fromIndex);
        }
    }

    public void SwapItemSlots(int p_fromIndex, int p_toIndex) {
        InventoryItem temp = inventoryItems[p_fromIndex];
        inventoryItems[p_fromIndex] = inventoryItems[p_toIndex];
        inventoryItems[p_toIndex] = temp;
    }

    public void SplitItems(int p_fromIndex) {
        InventoryItem splitItem = inventoryItems[p_fromIndex];

        for (int i=0; i<inventoryItems.Count; i++) {
            if (inventoryItems[i].itemId == -1) {
                int splitCount = Mathf.FloorToInt(splitItem.itemCount / 2);
                splitItem.itemCount -= splitCount;

                inventoryItems[i].itemId = splitItem.itemId;
                inventoryItems[i].itemCount = splitCount;
                return;
            }
        }
        //Send error no vacant
    }

    public void DropItemSlot(int p_index) {
        InventoryItem item = inventoryItems[p_index];
        OnDropItems?.Invoke(item.itemId, item.itemCount);

        DestroyItemSlot(p_index);
    }

    public void DropEquipItem(InventoryItem p_item) {
        OnDropItems?.Invoke(p_item.itemId, p_item.itemCount);
        p_item.itemId = -1;
        p_item.itemCount = 0;
    }

    public void DestroyItemSlot(int p_index) {
        inventoryItems[p_index].itemId = -1;
        inventoryItems[p_index].itemCount = 0;
    }

    public void SortItems(SortType p_sortType) {
        List<InventoryItem> nonEmpty = inventoryItems.Where(i => i.itemId != -1).ToList();
        List<InventoryItem> empties = inventoryItems.Where(i => i.itemId == -1).ToList();
        List<InventoryItem> newList = new List<InventoryItem>();
        switch (p_sortType) {
            case SortType.Type:
                newList = nonEmpty.OrderBy(i => GameManager.instance.GetItemData(i.itemId).itemType)
                    .ThenByDescending(i => GameManager.instance.GetItemData(i.itemId).rarity).ToList();
                break;
            case SortType.Rarity:
                newList = nonEmpty.OrderByDescending(i => GameManager.instance.GetItemData(i.itemId).rarity)
                    .ThenBy(i => GameManager.instance.GetItemData(i.itemId).itemType).ToList();
                break;
            case SortType.Count:
                newList = nonEmpty.OrderByDescending(i => i.itemCount)
                    .ThenBy(i => GameManager.instance.GetItemData(i.itemId).itemType).ToList();
                break;
        }
        inventoryItems = newList.Concat(empties).ToList();
    }

}
