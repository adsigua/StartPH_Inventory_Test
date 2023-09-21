using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsMasterList", menuName = "Inventory Data/Items Master List", order = 0)]
public class ItemsMasterList : ScriptableObject {
    [SerializeField] private List<ItemData> m_itemDB;

    public List<float> raritySpawnWeights = new List<float>();
    public List<Color> rarityColors = new List<Color>();

    [ContextMenu("Load Items")]
    public void LoadItems() {
        m_itemDB = new List<ItemData>();

        List<ItemData> loadedItems = Resources.LoadAll<ItemData>("ItemData").OrderBy(item => item.itemID).ToList();

        if (loadedItems == null || loadedItems.Count == 0) {
            return;
        }

        List<ItemData> inRangeItems = loadedItems.Where(i => i.itemID != -1 && i.itemID < loadedItems.Count).ToList();
        List<ItemData> outOfRangeItems = loadedItems.Where(i => i.itemID != -1 && i.itemID >= loadedItems.Count).ToList();
        List<ItemData> noIDitems = loadedItems.Where(i => i.itemID <= -1).ToList();

        int noIdCounter = 0;
        for (int i = 0; i < loadedItems.Count; i++) {
            ItemData itemToAdd = inRangeItems.Find(item => item.itemID == i);

            if (itemToAdd != null) {
                m_itemDB.Add(itemToAdd);
            } else if (noIdCounter < noIDitems.Count) {
                itemToAdd = noIDitems[noIdCounter++];
                itemToAdd.itemID = i;
                m_itemDB.Add(itemToAdd);
            }
        }

        foreach(ItemData item in outOfRangeItems) {
            m_itemDB.Add(item);
        }
    }

    public ItemData GetRandomItemWeighted() {
        float totalWeights = raritySpawnWeights.Sum();
        float randVal = Random.value;
        float currWeight = 0;
        for (int i=0; i<raritySpawnWeights.Count; i++) {
            currWeight += raritySpawnWeights[i] / (float)totalWeights;
            if (randVal <= currWeight) {
                List<ItemData> rarityItems = m_itemDB.Where(item => item.rarity == (ItemData.RarityType)i).ToList();
                if(rarityItems.Count > 0) {
                    return rarityItems[Random.Range(0, rarityItems.Count)];
                }
            }
        }
        List<ItemData> commonItems = m_itemDB.Where(item => item.rarity == ItemData.RarityType.Common).ToList();
        return commonItems[Random.Range(0, commonItems.Count)];
    }

    public ItemData GetItemData(int p_id) {
        return m_itemDB.Find(item => item.itemID == p_id);
    }
}
