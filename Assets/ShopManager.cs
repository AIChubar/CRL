using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [HideInInspector]
    public ItemButton currentItem;
    public bool inputDisabled;
    public GameObject itemButtonsParent;
    public static ShopManager instance;
    
    public TMP_Text instructionText;
    public TMP_Text goldText;
    
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

    public void UpdateUI()
    {
        goldText.text = "Gold: $" + GameManager.instance.gameData.gold.ToString();
    }

    void Start()
    {
        goldText.text = "Gold: $" + GameManager.instance.gameData.gold.ToString();

        GameData gameData = GameManager.instance.gameData;

        items.Enqueue(new Item("Lucky Charm",3, new List<StatModifier>
        {
            new(0.1f, StatModType.PercentAdd, gameData.wildLuck, "Wild Luck")
        }
        ));

        items.Enqueue(new Item("Jackpot Boost", 5,new List<StatModifier>
        {
            new(0.2f, StatModType.PercentMult, gameData.payoutMult, "Payout Mult"),
            new(50f, StatModType.Flat, gameData.payoutBonus, "Payout Bonus"),

        }));

        items.Enqueue(new Item("Bonus Payout", 3,new List<StatModifier>
        {
            new(50f, StatModType.Flat, gameData.payoutBonus, "Payout Bonus")
        }
        ));

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
            if (items.Count > 0)
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
        if (currentItem.item.price > GameManager.instance.gameData.gold)
        {
            instructionText.text = "Not enough gold!";
            return;

        }
        else
        {
            GameManager.instance.gameData.gold -= currentItem.item.price;
        }
        Item item = currentItem.item;
        GameManager.instance.gameData.playerItems.Add(item);

        // Apply the item's stat modifiers
        item.ApplyModifiers();

        Destroy(currentItem.gameObject);
        currentItem = null;
        UpdateUI();
    }

    public void SetCurrentItem(ItemButton item)
    {
        currentItem = item;
    }
}


