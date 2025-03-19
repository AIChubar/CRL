using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;


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
    public float RTP = 0.95f;
    public float baseMoney = 2000;
    public float money;
    public int spinsLeft = 10;
    public int wagerLeft = 2000;
    public int targetMoney = 1500;
    public int betAmount = 10;
    public int changePrice = 5;
    public int currentLevel = 1;
    public int gold = 0;

    [SerializeField]public CharacterStat wildLuck = new CharacterStat(1f);
    [SerializeField]public CharacterStat payoutMult = new CharacterStat(1f);
    [SerializeField]public CharacterStat payoutBonus = new CharacterStat(0);
    
    public List<Item> playerItems =  new List<Item>();
    public GameData()
    {
        money = baseMoney;
    }
}

