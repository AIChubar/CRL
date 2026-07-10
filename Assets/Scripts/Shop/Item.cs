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
}
    
