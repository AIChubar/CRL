using UnityEngine;

[CreateAssetMenu(fileName = "ColumnRollConsumableItem", menuName = "Item/Column Roll Consumable Item")]
public class ColumnRollConsumableItem : ConsumableItem
{
    
    protected override void ApplyEffect()
    {
        Debug.Log("Applying Column Roll Effect");
    }
}
