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
    public int tokens;
    public float wildLuck;
    public float payoutMult;
    public float payoutBonus;
    public int baseSpins;
    public List<string> passiveItemNames = new List<string>();
    public string slotConfigName;
    public List<ConsumableItemData> consumableItemData = new List<ConsumableItemData>();
    public List<int> columnBuffs = new List<int>();
    public string shopConfigName;
    public GameDataSerializable(GameData data)
    {
        tokens = data.tokens;
        slotConfigName = data.slotConfig.name;
        shopConfigName = data.shopConfig.name;
        RTP = data.RTP;
        baseMoney = data.baseMoney;
        money = data.money;
        spinsLeft = data.spinsLeft;
        targetMoney = data.targetMoney;
        betAmount = data.betAmount;
        changePrice = data.changePrice;
        currentLevel = data.currentLevel;
        gold = data.gold;
        baseSpins = data.baseSpins;
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
                    savedItemName = item.name,
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
        data.targetMoney = targetMoney;
        data.betAmount = betAmount;
        data.changePrice = changePrice;
        data.currentLevel = currentLevel;
        data.gold = gold;
        data.tokens = tokens;
        data.wildLuck = new CharacterStat(wildLuck);
        data.payoutMult = new CharacterStat(payoutMult);
        data.payoutBonus = new CharacterStat(payoutBonus);
        data.baseSpins = baseSpins;
        data.passiveItems = new List<PassiveItem>();
        foreach (var savedItemName in passiveItemNames)
        {
            PassiveItem loadedItem = Resources.Load<PassiveItem>($"Shop/PassiveItems/{savedItemName}");
            if (loadedItem != null)
                data.passiveItems.Add(loadedItem);
            else
                Debug.LogWarning($"Passive item '{savedItemName}' not found in Resources folder!");
        }
        data.consumableItems = new List<ConsumableItem>();
        foreach (var itemData in consumableItemData)
        {
            ConsumableItem runtimeItem = GetRuntimeConsumable(itemData.savedItemName, itemData.savedCharges);
            if (runtimeItem != null)
                data.consumableItems.Add(runtimeItem);
            else
                Debug.LogWarning($"Consumable item '{itemData.savedItemName}' not found in Resources folder!");
        }

        SlotConfig slotConfigLoaded = Resources.Load<SlotConfig>($"Slots/{slotConfigName}");
        if (slotConfigLoaded != null)
            data.slotConfig = slotConfigLoaded;
        else
            Debug.LogWarning($"Slot '{slotConfigName}' not found in Resources folder!");
        ShopConfig shopConfigLoaded = Resources.Load<ShopConfig>($"Shop/{shopConfigName}");
        if (shopConfigLoaded != null)
            data.shopConfig = shopConfigLoaded;
        else
            Debug.LogWarning($"Shop '{shopConfigName}' not found in Resources folder!");
        data.ApplyAllModifiers();
    }

    private ConsumableItem GetRuntimeConsumable(string savedItemName, int savedCharges)
    {
        ConsumableItem template = Resources.Load<ConsumableItem>($"Shop/ConsumableItems/{savedItemName}");

        if (template == null)
        {
            Debug.LogWarning($"Consumable item '{savedItemName}' could not be loaded from Resources.");
            return null;
        }

        ConsumableItem clone = ScriptableObject.Instantiate(template);
        clone.charges = savedCharges;
        clone.name = template.name;
        return clone;
    }
}

[System.Serializable]
public class ConsumableItemData
{
    public string savedItemName;
    public int savedCharges;
}
