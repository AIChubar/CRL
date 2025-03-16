using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing all data that needs to be saved.
/// </summary>
[System.Serializable]
public class GameData
{
    public float RTP = 0.95f;
    public float money = 2000;
    public int spinsLeft = 10;
    public int wagerLeft = 2000;
    public int targetMoney = 1500;
    public int betAmount = 10;
    public int changePrice = 5;
    public int currentLevel = 1;
    
    [SerializeField]public CharacterStat wildLuck = new CharacterStat(1f);
    [SerializeField]public CharacterStat payoutCoef = new CharacterStat(1f);
    [SerializeField]public CharacterStat payoutBonus = new CharacterStat(0);
    
    public List<Item> playerItems =  new List<Item>();
    public GameData()
    {
        
    }
}

