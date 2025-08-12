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
    
    public TMP_Text instructionText;
    public TMP_Text goldText;
    public TMP_Text tokensOfferText;
    public TMP_Text tokensText;
    public TMP_Text rerollText;
    
    private int rerollCost;
    private int tokensToBuy;
    private int tokensGold;
    
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button menuButton;

    [SerializeField] private Button rerollButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button buyTokensButton;

    [SerializeField] private GameObject itemButtonPrefab;
    public GameData gameData;
    //[SerializeField] private ShopConfig shopConfig;
    private List<Item> availableItems; // Items currently in the pool
    private List<Item> currentShopItems; // Currently displayed shop items

    
    
    private void Start()
    {
        nextLevelButton.onClick.AddListener(NextLevel);
        rerollButton.onClick.AddListener(Reroll);
        buyButton.onClick.AddListener(BuyItem);
        buyTokensButton.onClick.AddListener(BuyTokens);
        menuButton.onClick.AddListener(OnMenuButtonClick);
        availableItems = new List<Item>(gameData.shopConfig.availableItems); // Initialize the pool
        currentShopItems = new List<Item>();
        rerollCost = 1;
        UpdateUI();
        RerollItems();
        RerollTokens();
    }

    private void Reroll()
    {
        if (gameData.gold < rerollCost)
        {
            instructionText.text = "Not enough gold!";
            return;
        }
        gameData.gold -= rerollCost;
        rerollCost++;
        RerollItems(); // Generate initial shop items
        RerollTokens();
        UpdateUI();
    }

    private void RerollItems()
    {
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

    private void RerollTokens()
    {
        tokensToBuy = GameManager.instance.rngManager.NextInt(gameData.currentLevel, gameData.currentLevel*2 + 1);
        tokensGold = GameManager.instance.rngManager.NextInt(gameData.currentLevel, gameData.currentLevel*3 + 2);
        tokensOfferText.text = "Tokens offer: \n" + tokensToBuy + "T for " + tokensGold +" Gold";;
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
        ib.SetButton(item, this);
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
            ConsumableItem newConsumable = Instantiate(consumable);
            newConsumable.name = consumable.name; 
            gameData.consumableItems.Add(newConsumable);
        }
        else if (passive != null)
        {
            gameData.passiveItems.Add(passive);
        }
        gameData.gold -= currentItem.item.price;
        Destroy(currentItem.gameObject);
        currentShopItems.Remove(currentItem.item);
        gameData.shopConfig.availableItems.Remove(currentItem.item);
        currentItem = null;

        UpdateUI();
    }

    private void BuyTokens()
    {
        if (gameData.gold < tokensGold)
        {
            instructionText.text = "Not enough gold!";
        }
        else
        {
            gameData.gold -= tokensGold;
            gameData.initialLevelTokens += tokensToBuy;
            instructionText.text = "Tokens bought!";
            RerollTokens();
        }

        UpdateUI();
    }

    public void NextLevel()
    {
        gameData.ApplyAllModifiers();
        gameData.tokens = gameData.initialLevelTokens;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void OnMenuButtonClick()
    {
        gameData.ApplyAllModifiers();
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    public void SetCurrentItem(ItemButton item)
    {
        currentItem = item;
    }

    public void UpdateUI()
    {
        goldText.text =   $"Gold: {gameData.gold}";
        tokensText.text =  $"Tokens: {gameData.initialLevelTokens}T";
        rerollText.text =  $"Reroll: {rerollCost} Gold";
    }
}
