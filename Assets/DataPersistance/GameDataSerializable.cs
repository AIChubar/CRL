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

    public List<string> playerItemNames = new List<string>(); // Store item names

    public GameDataSerializable(GameData data)
    {
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
        
        wildLuck = data.wildLuck.Value;
        payoutMult = data.payoutMult.Value;
        payoutBonus = data.payoutBonus.Value;

        foreach (var item in data.playerItems)
        {
            playerItemNames.Add(item.itemName); // Save only the item name
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

        data.playerItems.Clear();
        foreach (var itemName in playerItemNames)
        {
            Item loadedItem = Resources.Load<Item>($"Items/{itemName}");
            if (loadedItem != null)
            {
                data.playerItems.Add(loadedItem);
            }
            else
            {
                Debug.LogWarning($"Item '{itemName}' not found in Resources folder!");
            }
        }
    }
}
