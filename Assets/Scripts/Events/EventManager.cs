using UnityEngine;

public class EventManager
{
    public EventController<ConsumableItemType> OnConsumableItemUsed { get; private set; }
    public EventController OnSpinAnimationEnd { get; private set; }

    public EventManager()
    {
        OnConsumableItemUsed = new EventController<ConsumableItemType>();
        OnSpinAnimationEnd = new EventController();

    }

}
