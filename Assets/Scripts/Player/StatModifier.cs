using UnityEngine;

public enum StatType
{
    WildLuck, 
    PayoutMult, 
    PayoutBonus,
    ColumnBuff
}

public enum StatModType
{
    Flat,
    PercentAdd,
    PercentMult,
}

[CreateAssetMenu(fileName = "StatModifier", menuName = "Stat Modifier")]
public class StatModifier : ScriptableObject
{
    public StatType StatType; // New field to specify which stat it modifies
    public float Value;
    public StatModType StatModType;
    public int Order;
    public string Description;

    public bool IsColumnSpecific; // Checkbox to enable the column index
    public int ColumnIndex = -1; // Column index, defaults to -1

    [HideInInspector] public bool IsTemporary = false; 
}