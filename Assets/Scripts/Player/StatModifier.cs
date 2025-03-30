using UnityEngine;


public enum StatModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
}


[CreateAssetMenu(fileName = "StatModifier", menuName = "Stat Modifier")]
public class StatModifier : ScriptableObject
{
    public StatType StatType; // New field to specify which stat it modifies
    public float Value;
    public StatModType Type;
    public int Order;
    public string Description;

    public StatModifier(StatType affectedStat, float value, StatModType type, int order = 0, string description = "")
    {
        StatType = affectedStat;
        Value = value;
        Type = type;
        Order = order;
        Description = description;
    }
}
