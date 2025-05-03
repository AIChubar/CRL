using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Shop_Config", menuName = "Shop Config")]

public class ShopConfig : ScriptableObject
{
    public List<Item> allItems; // Master list of all items
    public List<Item> availableItems; // List of items that can be purchased

    public void Reset()
    {
        availableItems = new List<Item>(allItems);
    }
}
