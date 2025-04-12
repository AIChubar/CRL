using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    [HideInInspector] public ItemButton currentItem;
    public bool inputDisabled;
    
    public GameObject itemButtonsParent;
    public static ShopManager instance;
    
    public TMP_Text instructionText;
    public TMP_Text goldText;
    
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button menuButton;

    [SerializeField] private Button rerollButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject itemButtonPrefab;
    public GameData gameData;
    public List<Item> allItems; // Master list of all items
    private List<Item> availableItems; // Items currently in the pool
    private List<Item> currentShopItems; // Currently displayed shop items

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        nextLevelButton.onClick.AddListener(NextLevel);
        rerollButton.onClick.AddListener(RerollItems);
        buyButton.onClick.AddListener(BuyItem);
        menuButton.onClick.AddListener(OnMenuButtonClick);
        availableItems = new List<Item>(allItems); // Initialize the pool
        currentShopItems = new List<Item>();

        UpdateUI();
        RerollItems(); // Generate initial shop items
    }

    public void RerollItems()
    {
        // Return previous shop items to the pool
        availableItems.AddRange(currentShopItems);
        currentShopItems.Clear();

        // Remove old UI buttons
        foreach (Transform child in itemButtonsParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Select 3 new items with weighted probability
        for (int i = 0; i < 3; i++)
        {
            if (availableItems.Count == 0) break;

            Item selectedItem = GetWeightedRandomItem();
            if (selectedItem != null)
            {
                currentShopItems.Add(selectedItem);
                availableItems.Remove(selectedItem); // Remove from available pool
                AddButton(selectedItem);
            }
        }
    }

    private Item GetWeightedRandomItem()
    {
        if (availableItems.Count == 0) return null;

        // Create a weighted list
        Dictionary<Item, float> weightedItems = new Dictionary<Item, float>();
        foreach (Item item in availableItems)
        {
            weightedItems[item] = item.GetWeight(); // Use rarity weight
        }

        // Weighted random selection
        float totalWeight = weightedItems.Values.Sum();
        float randomValue = Random.Range(0, totalWeight);

        foreach (var pair in weightedItems)
        {
            randomValue -= pair.Value;
            if (randomValue <= 0) return pair.Key;
        }

        return availableItems[0]; // Fallback
    }

    public void AddButton(Item item)
    {
        GameObject go = Instantiate(itemButtonPrefab, itemButtonsParent.transform);
        ItemButton ib = go.GetComponent<ItemButton>();
        ib.SetButton(item);
        go.transform.localScale = Vector3.one;
    }

    public void BuyItem()
    {
        if (currentItem == null)
        {
            instructionText.text = "No item selected.";
            return;
        }

        if (currentItem.item.price > gameData.gold)
        {
            instructionText.text = "Not enough gold!";
            return;
        }

        ConsumableItem consumable = currentItem.item as ConsumableItem;
        PassiveItem passive = currentItem.item as PassiveItem;

        if (consumable != null)
        {
            gameData.consumableItems.Add(consumable);
        }
        else if (passive != null)
        {
            gameData.passiveItems.Add(passive);
        }
        Destroy(currentItem.gameObject);
        currentShopItems.Remove(currentItem.item); // Remove from shop items
        currentItem = null;

        UpdateUI();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void OnMenuButtonClick()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    public void SetCurrentItem(ItemButton item)
    {
        currentItem = item;
    }

    public void UpdateUI()
    {
        goldText.text = "Gold: $" + gameData.gold;
    }
}
