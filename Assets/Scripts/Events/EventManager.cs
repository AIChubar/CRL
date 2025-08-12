using UnityEngine;

public class EventManager
{
    public EventController<ConsumableItemType> OnConsumableItemUsed { get; private set; }
    public EventController<AnimationType> OnCoroutineEnd { get; private set; }
    
    public EventController OnSpinButtonClick { get; private set; }

    public EventManager()
    {
        OnConsumableItemUsed = new EventController<ConsumableItemType>();
        OnCoroutineEnd = new EventController<AnimationType>();
        OnSpinButtonClick = new EventController();
    }

}
