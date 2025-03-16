using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    
    [HideInInspector] public Image image;

    public TextMeshProUGUI text;

    public Item item;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetButton(Item _item)
    {
        item = _item;
        text.text = item.name;
        image = GetComponent<Image>();
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
    
    
}
public class Item
{
    public string name;
    public List<StatModifier> statModifiers;

    public Item(string _name, List<StatModifier> modifiers)
    {
        name = _name;
        statModifiers = modifiers;
    }

    public void ApplyModifiers()
    {
        foreach (StatModifier mod in statModifiers)
        {
            if (mod.Source is CharacterStat targetStat)
            {
                targetStat.AddModifier(mod);
            }
        }
    }

    public void RemoveModifiers()
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

