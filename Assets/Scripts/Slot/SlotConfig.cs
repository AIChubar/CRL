using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotConfig", menuName = "Slot Config", order = 1)]
public class SlotConfig : ScriptableObject
{
    
    [Header("Symbols Settings")]
    public List<Symbol> symbols = new List<Symbol>();

    [Header("Grid Settings")]
    public int rows = 3;
    public int columns = 3;
}