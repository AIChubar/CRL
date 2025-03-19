using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    
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
    
    public void SetButton(Item item)
    {
        this.item = item;
        InitializeStatModifiers();
        nameText.text = item.itemName;
        priceText.text = item.price.ToString();
        descriptionText.text = "";
        for (int i = 0; i < item.statModifiers.Count; i++)
        {
            descriptionText.text += item.statModifiers[i].Description + " " + item.statModifiers[i].Value.ToString() + "\n";
        }
        image = GetComponent<Image>();
    }

    public void InitializeStatModifiers()
    {
        foreach (var stat in item.itemStats)
        {
            object source = stat.statType switch
            {
                StatType.PayoutBonus  => GameManager.instance.gameData.payoutBonus,
                StatType.PayoutMult => GameManager.instance.gameData.payoutMult,
                StatType.WildLuck   => GameManager.instance.gameData.wildLuck,
                _                => stat // Default fallback
            };
            item.statModifiers.Add(new StatModifier(stat.value, stat.statModType, source, nameof(stat.statType)));
        }
    }
    
    public void OnClicked()
    {
        if (ShopManager.instance.inputDisabled)
            return;
        if (ShopManager.instance.currentItem != null)
        {
            ShopManager.instance.currentItem.image.color = Color.white;
        }
        ShopManager.instance.SetCurrentItem(this);
        image.color = new Color32(245, 124, 124, 255);
    }
    
    public void ApplyModifiers(List<StatModifier> statModifiers)
    {
        foreach (StatModifier mod in statModifiers)
        {
            if (mod.Source is CharacterStat targetStat)
            {
                targetStat.AddModifier(mod);
            }
        }
    }
    
    public void RemoveModifiers(List<StatModifier> statModifiers)
    {
        foreach (StatModifier mod in statModifiers)
        {
            if (mod.Source is CharacterStat targetStat)
            {
                targetStat.RemoveModifier(mod);
            }
        }
    }
}




