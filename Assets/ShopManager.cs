using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [HideInInspector]
    public ItemButton currentItem;
    public bool inputDisabled;
    public GameObject itemButtonsParent;
    public static ShopManager instance;
    
    
    
    [HideInInspector]
    [UnityEngine.Tooltip("Contains all available items.")]
    public Queue<Item> items = new Queue<Item>();
    
    [UnityEngine.Tooltip("Prefab for UI Item object representing the gameWorld.")]
    public GameObject itemButtonPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        GameData gameData = GameManager.instance.gameData;

        items.Enqueue(new Item("Lucky Charm", new List<StatModifier>
        {
            new(0.1f, StatModType.PercentAdd, gameData.wildLuck)
        }));

        items.Enqueue(new Item("Jackpot Boost", new List<StatModifier>
        {
            new(0.2f, StatModType.PercentMult, gameData.payoutCoef)
        }));

        items.Enqueue(new Item("Bonus Payout", new List<StatModifier>
        {
            new(50f, StatModType.Flat, gameData.payoutBonus)
        }));

        RerollItems();
    }

    public void RerollItems()
    {
        foreach (ItemButton item in itemButtonsParent.GetComponentsInChildren<ItemButton>())
        {
            items.Enqueue(item.item);
            Destroy(item.gameObject);
        }
        for (int i = 0; i < 3; i++)
        {
            AddButton(items.Dequeue());
        }
    }
    // Update is called once per frame
    public void AddButton(Item item)
    {
        GameObject go = Instantiate(itemButtonPrefab, itemButtonsParent.transform);
        ItemButton ib = go.GetComponent<ItemButton>();
        ib.SetButton(item);
        go.transform.localScale = new Vector3(1, 1, 1);
    }

    public void BuyItem()
    {
        Item item = currentItem.item;
        GameManager.instance.gameData.playerItems.Add(item);

        // Apply the item's stat modifiers
        item.ApplyModifiers();

        Destroy(currentItem.gameObject);
        currentItem = null;
    }

    public void SetCurrentItem(ItemButton item)
    {
        currentItem = item;
    }
}


