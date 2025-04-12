using System.Collections.Generic;
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

    public List<string> passiveItemNames = new List<string>();
    public string slotConfigName;
    public List<ConsumableItemData> consumableItemData = new List<ConsumableItemData>();
    public List<int> columnBuffs = new List<int>();

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

        passiveItemNames = new List<string>();
        foreach (var passive in data.passiveItems)
        {
            passiveItemNames.Add(passive.name);
        }

        consumableItemData = new List<ConsumableItemData>();
        foreach (var item in data.consumableItems)
        {
            if (item != null)
            {
                consumableItemData.Add(new ConsumableItemData
                {
                    itemName = item.name,
                    savedCharges = item.charges
                });
            }
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

        data.passiveItems = new List<PassiveItem>();
        foreach (var itemName in passiveItemNames)
        {
            PassiveItem loadedItem = Resources.Load<PassiveItem>($"Items/PassiveItems/{itemName}");
            if (loadedItem != null)
                data.passiveItems.Add(loadedItem);
            else
                Debug.LogWarning($"Passive item '{itemName}' not found in Resources folder!");
        }
        data.consumableItems = new List<ConsumableItem>();
        foreach (var itemData in consumableItemData)
        {
            ConsumableItem runtimeItem = GetRuntimeConsumable(itemData.itemName, itemData.savedCharges);
            if (runtimeItem != null)
                data.consumableItems.Add(runtimeItem);
            else
                Debug.LogWarning($"Consumable item '{itemData.itemName}' not found in Resources folder!");
        }

        SlotConfig slotConfigLoaded = Resources.Load<SlotConfig>($"Slots/{slotConfigName}");
        if (slotConfigLoaded != null)
            data.slotConfig = slotConfigLoaded;
        else
            Debug.LogWarning($"Slot '{slotConfigName}' not found in Resources folder!");

        data.ApplyAllModifiers();
    }

    public static ConsumableItem GetRuntimeConsumable(string itemName, int savedCharges)
    {
        ConsumableItem template = Resources.Load<ConsumableItem>($"Items/ConsumableItems/{itemName}");

        if (template == null)
        {
            Debug.LogWarning($"Consumable item '{itemName}' could not be loaded from Resources.");
            return null;
        }

        ConsumableItem clone = ScriptableObject.Instantiate(template);
        clone.charges = savedCharges;
        return clone;
    }
}

[System.Serializable]
public class ConsumableItemData
{
    public string itemName;
    public int savedCharges;
}
