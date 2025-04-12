using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class containing all data that needs to be saved.
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    public float RTP;
    public float baseMoney;
    public float money;
    public int spinsLeft;
    public int wagerLeft;
    public int targetMoney;
    public int betAmount;
    public int changePrice;
    public int currentLevel;
    public int gold;

    [SerializeField] public CharacterStat wildLuck;
    [SerializeField] public CharacterStat payoutMult;
    [SerializeField] public CharacterStat payoutBonus;

    public List<PassiveItem> passiveItems;
    public List<ConsumableItem> consumableItems;

    [SerializeField] public SlotConfig slotConfig; // Assigned in Inspector
    
    [HideInInspector]public List<CharacterStat> columnBuffs = new List<CharacterStat>();

    public void CopyFrom(GameData other)
    {
        if (other == null) return;

        RTP = other.RTP;
        baseMoney = other.baseMoney;
        money = other.money;
        spinsLeft = other.spinsLeft;
        wagerLeft = other.wagerLeft;
        targetMoney = other.targetMoney;
        betAmount = other.betAmount;
        changePrice = other.changePrice;
        currentLevel = other.currentLevel;
        gold = other.gold;
        slotConfig = other.slotConfig;
        wildLuck = new CharacterStat(other.wildLuck.BaseValue);
        payoutMult = new CharacterStat(other.payoutMult.BaseValue);
        payoutBonus = new CharacterStat(other.payoutBonus.BaseValue);
        passiveItems = new List<PassiveItem>(other.passiveItems);
        consumableItems = new List<ConsumableItem>();
        foreach (ConsumableItem item in other.consumableItems)
            consumableItems.Add(Instantiate(item));
        columnBuffs.Clear();
        foreach (int val in slotConfig.columnBuffs)
            columnBuffs.Add(new CharacterStat(val));
        RemoveAllModifiers();
        ApplyAllModifiers();
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