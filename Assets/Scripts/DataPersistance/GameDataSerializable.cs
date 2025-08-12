using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataSerializable
{
    public float RTP;
    public float baseMoney;
    public float money;
    public int spinsLeft;
    public int targetMoney;
    public int betAmount;
    public List<int> changePriceProgression;
    public int wildPrice;
    public int currentLevel;
    public int gold;
    public int tokens;
    public int initialLevelTokens;
    public float wildLuck;
    public float payoutMult;
    public float payoutBonus;
    public int baseSpins;
    public List<string> passiveItemNames = new List<string>();
    public string slotConfigName;
    public List<ConsumableItemData> consumableItemData = new List<ConsumableItemData>();
    public string shopConfigName;
    public List<int> levelsTargetMoney;
    //public float animationSpeed;
    public GameDataSerializable(GameData data)
    {
        //animationSpeed = data.animationSpeed;
        initialLevelTokens = data.initialLevelTokens;
        wildPrice = data.wildPrice;
        tokens = data.tokens;
        slotConfigName = data.slotConfig.name;
        shopConfigName = data.shopConfig.name;
        RTP = data.RTP;
        baseMoney = data.baseMoney;
        money = data.money;
        spinsLeft = data.spinsLeft;
        targetMoney = data.targetMoney;
        betAmount = data.betAmount;
        changePriceProgression = data.changePriceProgression;
        currentLevel = data.currentLevel;
        gold = data.gold;
        baseSpins = data.baseSpins;
        wildLuck = data.wildLuck.BaseValue;
        payoutMult = data.payoutMult.BaseValue;
        payoutBonus = data.payoutBonus.BaseValue;
        passiveItemNames = new List<string>();
        levelsTargetMoney = data.levelsTargetMoney;
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
        //data.animationSpeed = animationSpeed;
        data.initialLevelTokens = initialLevelTokens;
        data.wildPrice = wildPrice;
        data.RTP = RTP;
        data.baseMoney = baseMoney;
        data.money = money;
        data.spinsLeft = spinsLeft;
        data.targetMoney = targetMoney;
        data.betAmount = betAmount;
        data.changePriceProgression = changePriceProgression;
        data.currentLevel = currentLevel;
        data.gold = gold;
        data.tokens = tokens;
        data.wildLuck = new CharacterStat(wildLuck);
        data.payoutMult = new CharacterStat(payoutMult);
        data.payoutBonus = new CharacterStat(payoutBonus);
        data.baseSpins = baseSpins;
        data.passiveItems = new List<PassiveItem>();
        data.levelsTargetMoney = levelsTargetMoney;
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
