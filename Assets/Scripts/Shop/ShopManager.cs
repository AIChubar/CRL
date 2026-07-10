using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public bool inputDisabled;
    
    public GameObject itemButtonsParent;
    
    public TMP_Text instructionText;
    public GameObject instructionPanel;
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
        buyTokensButton.onClick.AddListener(BuyTokens);
        menuButton.onClick.AddListener(OnMenuButtonClick);
        availableItems = new List<Item>(gameData.availableShopItems); // Initialize the pool
        currentShopItems = new List<Item>();
        rerollCost = gameData.shopConfig.baseRerollCost;
        SetInstruction("");
        UpdateUI();
        RerollItems(gameData.shopConfig.guaranteedItem);
        RerollTokens();
    }

    private void SetInstruction(string text)
    {
        instructionText.text = text;
        if (instructionPanel != null)
            instructionPanel.SetActive(!string.IsNullOrEmpty(text));
    }

    private void Reroll()
    {
        if (gameData.gold < rerollCost)
        {
            SetInstruction("Not enough gold!");
            return;
        }
        gameData.gold -= rerollCost;
        rerollCost += gameData.shopConfig.rerollCostIncrement;
        RerollItems(); // Generate initial shop items
        RerollTokens();
        UpdateUI();
    }

    private void RerollItems(Item guaranteedItem = null)
    {
        availableItems.AddRange(currentShopItems);
        currentShopItems.Clear();

        // Remove old UI buttons
        foreach (Transform child in itemButtonsParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Reserve one slot for the guaranteed item if it's still available
        if (guaranteedItem != null && availableItems.Contains(guaranteedItem))
        {
            currentShopItems.Add(guaranteedItem);
            availableItems.Remove(guaranteedItem);
            AddButton(guaranteedItem);
        }

        // Fill the remaining slots with weighted random items
        while (currentShopItems.Count < gameData.shopConfig.shopSize)
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
        ShopConfig config = gameData.shopConfig;
        tokensToBuy = GameManager.instance.rngManager.NextInt(gameData.currentLevel, gameData.currentLevel * config.tokenAmountLevelMult + config.tokenAmountConstant);
        tokensGold = GameManager.instance.rngManager.NextInt(gameData.currentLevel, gameData.currentLevel * config.tokenPriceLevelMult + config.tokenPriceConstant);
        tokensOfferText.text = "Tokens offer: \n" + tokensToBuy + "T for " + tokensGold +" Gold";;
    }

    private Item GetWeightedRandomItem()
    {
        if (availableItems.Count == 0) return null;

        // Create a weighted list
        Dictionary<Item, float> weightedItems = new Dictionary<Item, float>();
        foreach (Item item in availableItems)
        {
            weightedItems[item] = gameData.shopConfig.GetWeight(item.rarity); // Use rarity weight
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
        ShopItem ib = go.GetComponent<ShopItem>();
        ib.SetButton(item, this);
        go.transform.localScale = Vector3.one;
    }

    public void BuyItem(ShopItem itemButton)
    {
        if (itemButton == null)
        {
            SetInstruction("No item selected.");
            return;
        }

        if (itemButton.item.price > gameData.gold)
        {
            SetInstruction("Not enough gold!");
            return;
        }

        ConsumableItem consumable = itemButton.item as ConsumableItem;
        PassiveItem passive = itemButton.item as PassiveItem;

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
        gameData.gold -= itemButton.item.price;
        currentShopItems.Remove(itemButton.item);
        gameData.availableShopItems.Remove(itemButton.item);
        Destroy(itemButton.gameObject);

        UpdateUI();
    }

    private void BuyTokens()
    {
        if (gameData.gold < tokensGold)
        {
            SetInstruction("Not enough gold!");
        }
        else
        {
            gameData.gold -= tokensGold;
            gameData.initialLevelTokens += tokensToBuy;
            SetInstruction("Tokens bought!");
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

    public void UpdateUI()
    {
        goldText.text =   $"{gameData.gold}";
        tokensText.text =  $"{gameData.initialLevelTokens}T";
        rerollText.text =  $"Reroll: {rerollCost} Gold";
    }
}
