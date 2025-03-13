using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [HideInInspector]
    public ItemButton currentItem;
    public bool inputDisabled;
    public GameObject itemButtonsParent;
    public static ShopManager instance  { get; private set; }
    
    
    
    [HideInInspector]
    [UnityEngine.Tooltip("Contains all available items.")]
    public List<Item> items = new List<Item>();
    
    [UnityEngine.Tooltip("Prefab for UI Item object representing the gameWorld.")]
    public GameObject itemButtonPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
       
    }
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            items.Add(new Item(i.ToString()));
        }

        RerollItems();
    }

    public void RerollItems()
    {
        foreach (ItemButton item in itemButtonsParent.GetComponentsInChildren<ItemButton>())
        {
            items.Add(item.item);
            Destroy(item.gameObject);
        }
        for (int i = 0; i < 3; i++)
        {
            AddButton(items[i]);
            items.RemoveAt(i);
        }
    }
    // Update is called once per frame
    public void AddButton(Item item)
    {
        GameObject go = Instantiate(itemButtonPrefab, itemButtonsParent.transform);
        ItemButton ib = go.GetComponent<ItemButton>();
        ib.SetButton(item);
        go.transform.localScale = new Vector3(1, 1, 1);
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
