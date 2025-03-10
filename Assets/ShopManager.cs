using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public ItemButton currentItem;
    public bool inputDisabled;
    public GameObject itemButtonsParent;
    public static ShopManager instance  { get; private set; }
    
    [HideInInspector]
    [UnityEngine.Tooltip("Contains all available items.")]
    public List<ItemButton> items = new List<ItemButton>();
    
    [UnityEngine.Tooltip("Prefab for UI Item object representing the gameWorld.")]
    public GameObject itemButtonPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance.AddButton(new Item("one"));
        instance.AddButton(new Item("two"));
        instance.AddButton(new Item("three"));
    }

    // Update is called once per frame
    public void AddButton(Item item)
    {
        GameObject go = Instantiate(itemButtonPrefab, itemButtonsParent.transform);
        ItemButton ib = go.GetComponent<ItemButton>();
        ib.SetButton(item);
        go.transform.localScale = new Vector3(1, 1, 1);
        items.Add(ib);
        /*foreach (var button in go.GetComponentsInChildren<Button>())
        {
            items.Add(button);
        }*/

    }

    public void SetCurrentItem(ItemButton item)
    {
        currentItem = item;
    }
}

public class Item
{
    public string name;

    public Item(string _name)
    {
        name = _name;
    }
}
