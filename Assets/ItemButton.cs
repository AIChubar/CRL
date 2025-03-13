using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    
    [HideInInspector] public Image image;

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
