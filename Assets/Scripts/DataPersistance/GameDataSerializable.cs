using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameDataSerializable
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

    public float wildLuck;
    public float payoutMult;
    public float payoutBonus;

    public List<string> passiveItemNames = new List<string>(); // Store item names
    public List<string> consumableItemNames = new List<string>(); // Store item names
    public string slotConfigName;

    public List<int> columnBuffs = new List<int>();// Assigned in Inspector
    public GameDataSerializable(GameData data)
    {
        slotConfigName = data.slotConfig.name;
        RTP = data.RTP;
        baseMoney = data.baseMoney;
        money = data.money;
        spinsLeft = data.spinsLeft;
        wagerLeft = data.wagerLeft;
        targetMoney = data.targetMoney;
        betAmount = data.betAmount;
        changePrice = data.changePrice;
        currentLevel = data.currentLevel;
        gold = data.gold;
        
        wildLuck = data.wildLuck.BaseValue;
        payoutMult = data.payoutMult.BaseValue;
        payoutBonus = data.payoutBonus.BaseValue;
        /*foreach (var column in data.columnBuffs)
        {
            columnBuffs.Add((int)column.BaseValue); // Save only the item name
        }*/
        foreach (var item in data.passiveItems)
        {
            passiveItemNames.Add(item.name); // Save only the item name
        }
        foreach (var item in data.consumableItems)
        {
            consumableItemNames.Add(item.name); // Save only the item name
        }
    }

    public void ApplyToGameData(GameData data)
    {
        data.RTP = RTP;
        data.baseMoney = baseMoney;
        data.money = money;
        data.spinsLeft = spinsLeft;
        data.wagerLeft = wagerLeft;
        data.targetMoney = targetMoney;
        data.betAmount = betAmount;
        data.changePrice = changePrice;
        data.currentLevel = currentLevel;
        data.gold = gold;
        
        data.wildLuck = new CharacterStat(wildLuck);
        data.payoutMult = new CharacterStat(payoutMult);
        data.payoutBonus = new CharacterStat(payoutBonus);
        /*for (int i = 0; i < columnBuffs.Count; i++)
        {
            data.columnBuffs.Add(new CharacterStat(columnBuffs[i])); 
        }*/
        data.consumableItems = new List<ConsumableItem>();
        data.passiveItems = new List<PassiveItem>();

        data.consumableItems.Clear();
        data.passiveItems.Clear();
        foreach (var itemName in passiveItemNames)
        {
            PassiveItem loadedItem = Resources.Load<PassiveItem>($"Items/PassiveItems/{itemName}");
            if (loadedItem != null)
            {
                data.passiveItems.Add(loadedItem);
            }
            else
            {
                Debug.LogWarning($"Item '{itemName}' not found in Resources folder!");
            }
        }
        foreach (var itemName in consumableItemNames)
        {
            ConsumableItem loadedItem = Resources.Load<ConsumableItem>($"Items/ConsumableItems/{itemName}");
            if (loadedItem != null)
            {
                data.consumableItems.Add(loadedItem);
            }
            else
            {
                Debug.LogWarning($"Item '{itemName}' not found in Resources folder!");
            }
        }
        SlotConfig slotConfig = Resources.Load<SlotConfig>($"Slots/{slotConfigName}");
        if (slotConfig != null)
            data.slotConfig = slotConfig;
        else
        {
            Debug.LogWarning($"Slot '{slotConfigName}' not found in Resources folder!");
        }
        
        data.ApplyAllModifiers();
    }
    
    
}
