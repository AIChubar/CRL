using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class containing all data that needs to be saved.
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    public float RTP;
    [HideInInspector]public float baseMoney = 0;
    [HideInInspector]public float money = 0;
    [HideInInspector] public int spinsLeft;
    [HideInInspector] public int targetMoney;
    [HideInInspector]public int betAmount = 0;
    public int changePrice;
    public int tokens;
    public int currentLevel;
    public int gold;
    public int baseSpins;

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

    public void CopyFrom(GameData other)
    {
        if (other == null) return;

        tokens = other.tokens;
        RTP = other.RTP;
        baseMoney = other.baseMoney;
        money = other.money;
        spinsLeft = other.spinsLeft;
        targetMoney = other.targetMoney;
        betAmount = other.betAmount;
        changePrice = other.changePrice;
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

    public void Reset()
    {
        spinsLeft = baseSpins;
        money = baseMoney;
    }
    
    public void ApplyAllModifiers()
    {
        foreach (var item in passiveItems)
        {
            foreach (var mod in item.statModifiers)
            {
                if (mod.IsColumnSpecific)
                {
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
    }
    
    
    
}
