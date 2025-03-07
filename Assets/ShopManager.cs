using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public Item currentItem;
    public bool inputDisabled;

    public static ShopManager instance  { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentItem(Item item)
    {
        currentItem = item;
    }
}
