using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Item/Consumable Item")]
public class ConsumableItem : Item
{
    public ConsumableItemType consumableItemType;
    public int charges;
    public bool Use()
    {
        ApplyEffect();
        charges--;
        return charges < 1;
    }
    
    public override string GetDescription()
    {
        string effect = consumableItemType switch
        {
            ConsumableItemType.ColumnRoll => "Rerolls a column",
            ConsumableItemType.SymbolRoll => "Rerolls a symbol",
            ConsumableItemType.SymbolTypeRoll => "Rerolls all symbols of a type",
            _ => "Unknown effect"
        };

        return $"{effect} ({charges} charges)";
    }

    private void ApplyEffect() =>  GameManager.instance.eventManager.OnConsumableItemUsed.InvokeEvent(consumableItemType);
}
public enum ConsumableItemType
{
    ColumnRoll,
    SymbolRoll,
    SymbolTypeRoll
}