using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Shop_Config", menuName = "Shop Config")]

public class ShopConfig : ScriptableObject
{
    public List<Item> allItems; // Master list of all items — read-only at runtime
    public Item guaranteedItem; // Always offered when the shop is first initialized

    [Header("Shop layout")]
    [Min(1)] public int shopSize = 3; // Items offered per shop visit

    [Header("Reroll")]
    [Min(0)] public int baseRerollCost = 1;
    [Min(0)] public int rerollCostIncrement = 1;

    [Header("Token offers — NextInt(level, level * mult + constant)")]
    public int tokenAmountLevelMult = 2;
    public int tokenAmountConstant = 1;
    public int tokenPriceLevelMult = 3;
    public int tokenPriceConstant = 2;

    [Header("Rarity weights")]
    [Min(0f)] public float commonWeight = 40f;
    [Min(0f)] public float uncommonWeight = 30f;
    [Min(0f)] public float rareWeight = 18f;
    [Min(0f)] public float epicWeight = 8f;
    [Min(0f)] public float legendaryWeight = 4f;

    [Header("Level-completion reward")]
    [Min(0f)] public float incomeRate = 0.1f;   // Share of leftover money paid out as gold
    [Min(0)] public int incomeCap = 10;         // If raw income exceeds this, payout is capped to it
    [Min(0)] public int baseLevelReward = 5;
    [Min(0)] public int rewardPerLevel = 1;

    public float GetWeight(Item.ItemRarity rarity)
    {
        return rarity switch
        {
            Item.ItemRarity.Common => commonWeight,
            Item.ItemRarity.Uncommon => uncommonWeight,
            Item.ItemRarity.Rare => rareWeight,
            Item.ItemRarity.Epic => epicWeight,
            Item.ItemRarity.Legendary => legendaryWeight,
            _ => 1f
        };
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (allItems == null || allItems.Count == 0)
            Debug.LogWarning($"[ShopConfig] '{name}' has no items in allItems — use 'Populate with All Items'.", this);

        if (guaranteedItem != null && allItems != null && !allItems.Contains(guaranteedItem))
            Debug.LogWarning($"[ShopConfig] '{name}': guaranteedItem '{guaranteedItem.name}' is not in allItems.", this);
    }
#endif
}
