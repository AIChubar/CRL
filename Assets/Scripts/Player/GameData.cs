using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    WildLuck, 
    PayoutMult, 
    PayoutBonus
}

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

    public List<Item> playerItems;

    [SerializeField] public SlotConfig slotConfig; // Assigned in Inspector
    [SerializeField] public List<int> columnBuffs = new List<int>();

    public void CopyFrom(GameData other, SlotConfig slotConfig)
    {
        if (other == null) return;
        this.slotConfig = slotConfig;
        EnsureColumnBuffsSize(); // Ensure correct size

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

        wildLuck = new CharacterStat(other.wildLuck.Value);
        payoutMult = new CharacterStat(other.payoutMult.Value);
        payoutBonus = new CharacterStat(other.payoutBonus.Value);

        playerItems = new List<Item>(other.playerItems); // Assumes Item is a reference type
    }

    private void EnsureColumnBuffsSize()
    {
        if (slotConfig == null) return;

        int requiredSize = slotConfig.columns;

        if (columnBuffs.Count != requiredSize)
        {
            while (columnBuffs.Count < requiredSize)
                columnBuffs.Add(0); // Default value

            if (columnBuffs.Count > requiredSize)
                columnBuffs.RemoveRange(requiredSize, columnBuffs.Count - requiredSize);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        EnsureColumnBuffsSize();
    }
#endif
}