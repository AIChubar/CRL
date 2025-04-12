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

    private void ApplyEffect() =>  GameManager.instance.eventManager.OnConsumableItemUsed.InvokeEvent(consumableItemType);
}
public enum ConsumableItemType
{
    ColumnRoll,
    SymbolRoll,
    SymbolTypeRoll
}