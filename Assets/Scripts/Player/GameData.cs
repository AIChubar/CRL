using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class containing all data that needs to be saved.
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    [Min(0f)] public float RTP;
    [HideInInspector]public float baseMoney = 0;
    [HideInInspector]public float money = 0;
    [HideInInspector] public int spinsLeft;
    [HideInInspector] public int targetMoney;
    [HideInInspector]public int betAmount = 0;
    [Min(0)] public int wildPrice;
    [HideInInspector]public int tokens;
    [Min(0)] public int initialLevelTokens;
    [Min(0)] public int currentLevel;
    [Min(0)] public int gold;
    [Min(0)] public int baseSpins;
    [HideInInspector] public int changeSymbolUses; 
    public List<int> changePriceProgression;
    [HideInInspector]public float animationSpeed = 1.0f;

    [SerializeField] public CharacterStat wildLuck;
    [SerializeField] public CharacterStat payoutMult;
    [SerializeField] public CharacterStat payoutBonus;

    public List<PassiveItem> passiveItems;
    public List<ConsumableItem> consumableItems;

    [SerializeField] public SlotConfig slotConfig;
    [SerializeField] public List<int> levelsTargetMoney; // Assigned in Inspector
    [SerializeField] public ShopConfig shopConfig;
    [HideInInspector]public List<CharacterStat> columnBuffs = new List<CharacterStat>();
    [HideInInspector] public List<Item> availableShopItems = new List<Item>(); // Runtime shop pool for this run
    private readonly HashSet<PassiveItem> appliedPassives = new HashSet<PassiveItem>();

    public void InitShopPool()
    {
        availableShopItems = new List<Item>(shopConfig.allItems);
    }

    public void CopyFrom(GameData other)
    {
        if (other == null) return;
        initialLevelTokens = other.initialLevelTokens;
        tokens = other.tokens;
        RTP = other.RTP;
        baseMoney = other.baseMoney;
        money = other.money;
        spinsLeft = other.spinsLeft;
        targetMoney = other.targetMoney;
        betAmount = other.betAmount;
        changePriceProgression = other.changePriceProgression;
        wildPrice = other.wildPrice;
        currentLevel = other.currentLevel;
        gold = other.gold;
        shopConfig = other.shopConfig;
        slotConfig = other.slotConfig;
        baseSpins = other.baseSpins;
        wildLuck = new CharacterStat(other.wildLuck.BaseValue);
        payoutMult = new CharacterStat(other.payoutMult.BaseValue);
        payoutBonus = new CharacterStat(other.payoutBonus.BaseValue);
        passiveItems = new List<PassiveItem>(other.passiveItems);
        consumableItems = new List<ConsumableItem>();
        levelsTargetMoney = other.levelsTargetMoney;
        foreach (ConsumableItem item in other.consumableItems)
        {
            ConsumableItem cloneItem = Instantiate(item);
            cloneItem.name = item.name;
            consumableItems.Add(cloneItem);
            
        }
        columnBuffs.Clear();
        foreach (int val in slotConfig.columnBuffs)
            columnBuffs.Add(new CharacterStat(val));
        RemoveAllModifiers();
        ApplyAllModifiers();
    }
    
    public int GetCurrentChangePrice()
    {
        if (changeSymbolUses < changePriceProgression.Count)
            return changePriceProgression[changeSymbolUses];
        else
            return changePriceProgression[changePriceProgression.Count - 1];
    }

    public void ResetLevelState()
    {
        spinsLeft = baseSpins;
        money = baseMoney;
        tokens = initialLevelTokens;
        targetMoney = levelsTargetMoney[currentLevel];
    }

    
    public void ApplyAllModifiers()
    {
        foreach (var item in passiveItems)
        {
            if (!appliedPassives.Add(item)) // Add() returns false if already applied
                continue;
            foreach (var mod in item.statModifiers)
            {
                if (mod.IsColumnSpecific)
                {
                    if (!IsValidColumn(mod.ColumnIndex))
                    {
                        Debug.LogWarning($"[GameData] '{item.name}' column modifier index {mod.ColumnIndex} out of range (0..{columnBuffs.Count - 1}); skipping.");
                        continue;
                    }
                    columnBuffs[mod.ColumnIndex].AddModifier(mod);
                }
                else
                {
                    CharacterStat targetStat = mod.StatType switch
                    {
                        StatType.PayoutBonus  => payoutBonus,
                        StatType.PayoutMult => payoutMult,
                        StatType.WildLuck   => wildLuck,
                        _ => null
                    };
                    if (targetStat != null)
                        targetStat.AddModifier(mod);
                }
            }
        }
    }
    public void RemoveAllModifiers()
    {
        foreach (var item in passiveItems)
        {
            foreach (var mod in item.statModifiers)
            {
                if (mod.IsColumnSpecific)
                {
                    if (!IsValidColumn(mod.ColumnIndex))
                        continue;
                    columnBuffs[mod.ColumnIndex].RemoveModifier(mod);
                }
                else
                {
                    CharacterStat targetStat = mod.StatType switch
                    {
                        StatType.PayoutBonus  => payoutBonus,
                        StatType.PayoutMult => payoutMult,
                        StatType.WildLuck   => wildLuck,
                        _ => null
                    };
                    if (targetStat != null)
                        targetStat.RemoveModifier(mod);
                }
            }
        }
        appliedPassives.Clear();
    }


    private bool IsValidColumn(int columnIndex)
    {
        return columnIndex >= 0 && columnIndex < columnBuffs.Count;
    }

    public void OnSpinButtonClick()
    {
        changeSymbolUses = 0;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (levelsTargetMoney == null || levelsTargetMoney.Count == 0)
            Debug.LogWarning($"[GameData] '{name}' has no levelsTargetMoney entries — level reset will fail.", this);

        if (changePriceProgression == null || changePriceProgression.Count == 0)
            Debug.LogWarning($"[GameData] '{name}' has no changePriceProgression entries — symbol-change pricing will fail.", this);
    }
#endif
}
