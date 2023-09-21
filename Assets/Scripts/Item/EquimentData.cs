using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item_equipment_data", menuName = "Inventory Data/ItemData/Equipment", order = 1)]
public class EquimentData : ItemData
{
    public enum EquipmentType {
        Armor,
        Weapon
    }

    public override ItemType itemType => ItemType.Equipment;
    public EquipmentType equipmentType;
    public Material equipmentMaterial;

    public int attackDamage = 0;
    public int bonusHP = 0;
    public int armor = 0;

}
