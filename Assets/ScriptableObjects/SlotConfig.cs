using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotConfig", menuName = "Slot Machine/Slot Config", order = 1)]
public class SlotConfig : ScriptableObject
{
    [Header("Symbols Settings")]
    public string[] slotSymbols = { "🍎", "🍒", "🍋", "🍉", "🍌" };
    public string wildSymbol = "⭐";
    public float wildChance = 0.02f; // 2% шанс выпадения Wild

    [Header("Grid Settings")]
    public int rows = 3;
    public int columns = 3;
}