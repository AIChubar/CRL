using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    private ShopManager shopManager;
    [HideInInspector] public Image image;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI descriptionText;

    public Item item;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetButton(Item item, ShopManager shopManager)
    {
        this.shopManager = shopManager;
        this.item = item;
        nameText.text = item.itemName;
        priceText.text = item.price.ToString() + " Gold";
        descriptionText.text = item.GetDescription();
        
        image = GetComponent<Image>();
    }

 
    public void OnClicked()
    {
        if (shopManager.inputDisabled)
            return;
        if (shopManager.currentItem != null)
        {
            shopManager.currentItem.image.color = Color.white;
        }
        shopManager.SetCurrentItem(this);
        image.color = new Color32(245, 124, 124, 255);
    }
    
    
}




