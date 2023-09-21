using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    [SerializeField] private string m_playerId = "player_01";
    [SerializeField] private int m_startingHp = 5;
    [SerializeField] private int m_currentHp = 5;
    [SerializeField] private int m_startingMaxHp = 15;
    [SerializeField] private int m_currentMaxHp = 15;
    [SerializeField] private int m_baseDamage = 1;

    public PlayerInventoryData inventoryData = new PlayerInventoryData();

    public PlayerData() {
        m_currentHp = m_startingHp;
        m_currentMaxHp = m_startingMaxHp;
    }

    public int currentHp => m_currentHp;
    public int baseHp => m_currentMaxHp;
    public int baseDamage => m_baseDamage;

    public int addedDamage {
        get {
            int addDmg = 0;
            if (inventoryData.weaponSlot.itemId != -1) {
                addDmg = (GameManager.instance.GetItemData(inventoryData.weaponSlot.itemId) as EquimentData).attackDamage;
            }
            return addDmg;
        }
    }

    public int currentDamage => m_baseDamage + addedDamage;

    public int addedHP {
        get {
            int addHp = 0;
            if (inventoryData.armorSlot.itemId != -1) {
                addHp = (GameManager.instance.GetItemData(inventoryData.armorSlot.itemId) as EquimentData).bonusHP;
            }
            return addHp;
        }
    }

    public int currentMaxHp => m_currentMaxHp + addedHP;

    public void UpdateHP(int p_increase) {
        m_currentHp += p_increase;
        m_currentHp = Mathf.Clamp(m_currentHp, 0, currentMaxHp);
    }

    public void SavePlayerData() {
        string json = JsonUtility.ToJson(this, true);
        DBHelper.SaveToJson(json, m_playerId + "_data.json");
    }

    public void LoadPlayerData() {
        PlayerData data = DBHelper.LoadFromJson<PlayerData>(m_playerId + "_data.json");
        m_currentHp = data.currentHp;
        m_currentMaxHp = data.baseHp;
        inventoryData = data.inventoryData;
    }
    
    public void ClearPlayerData() {
        m_currentHp = m_startingHp;
        m_currentMaxHp = m_startingMaxHp;
        inventoryData = new PlayerInventoryData();
    }

}
