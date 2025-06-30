using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
//[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string itemName;
    //[HideInInspector]public string savedItemName;

    public int price;
    public ItemRarity rarity;
    public bool applied = false;
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    
    public virtual string GetDescription()
    {
        return "No description available.";
    }

    public float GetWeight()
    {
        return rarity switch
        {
            ItemRarity.Common => 40f,
            ItemRarity.Uncommon => 30f,
            ItemRarity.Rare => 18f,
            ItemRarity.Epic => 8f,
            ItemRarity.Legendary => 4f,
            _ => 1f
        };
    }

  
}
    
