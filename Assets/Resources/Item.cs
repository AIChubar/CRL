using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int price;
    public List<StatModifier> statModifiers;
    public ItemRarity rarity; // New rarity field
    
    public List<ItemStat> itemStats = new List<ItemStat>();

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public float GetWeight()
    {
        return rarity switch
        {
            ItemRarity.Common => 50f,
            ItemRarity.Uncommon => 30f,
            ItemRarity.Rare => 15f,
            ItemRarity.Epic => 4f,
            ItemRarity.Legendary => 1f,
            _ => 1f
        };
    }
}

public class ItemStat
{
    public StatType statType;
    public StatModType statModType;
    public float value;
}