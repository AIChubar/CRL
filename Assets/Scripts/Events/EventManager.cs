using UnityEngine;

public class EventManager
{
    public EventController<ConsumableItemType> OnConsumableItemUsed { get; private set; }
    public EventController OnSpinButtonClick { get; private set; }

    public EventManager()
    {
        OnConsumableItemUsed = new EventController<ConsumableItemType>();
        OnSpinButtonClick = new EventController();
    }
}
