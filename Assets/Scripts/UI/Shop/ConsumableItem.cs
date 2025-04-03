using UnityEngine;

//[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Item/Consumable Item")]
public class ConsumableItem : Item
{
    public int charges;
    public bool Use()
    {
        Debug.Log($"Using {itemName}");
        ApplyEffect();
        charges--;
        return charges < 1;
    }

    protected virtual void ApplyEffect()
    {
    }
}
