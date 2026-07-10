using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private ShopManager shopManager;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI descriptionText;
    public Button buyButton;

    [HideInInspector]
    public Item item;

    public void SetButton(Item item, ShopManager shopManager)
    {
        this.shopManager = shopManager;
        this.item = item;
        nameText.text = item.itemName;
        priceText.text = item.price.ToString() + " Gold";
        descriptionText.text = item.GetDescription();

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(Buy);
        }
    }

    public void Buy()
    {
        if (shopManager.inputDisabled)
            return;
        shopManager.BuyItem(this);
    }
}
