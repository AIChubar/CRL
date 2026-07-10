using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotConfig", menuName = "Slot Config", order = 1)]
public class SlotConfig : ScriptableObject
{
    
    [Header("Symbols Settings")]
    public List<Symbol> symbols = new List<Symbol>();

    [Header("Grid Settings")]
    [Min(1)] public int rows = 3;
    [Min(1)] public int columns = 3;
    
    [SerializeField] public List<int> columnBuffs = new List<int>();

    private void EnsureColumnBuffsSize()
    {

        int requiredSize = columns;

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
        if (symbols == null || symbols.Count == 0)
            Debug.LogWarning($"[SlotConfig] '{name}' has no symbols assigned.", this);
    }
#endif
}