using UnityEngine;

public class EventManager
{
    public EventController<int> OnMapSelected { get; private set; }

    public EventManager()
    {
        OnMapSelected = new EventController<int>();
    }

}
