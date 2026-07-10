using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableItemButton : MonoBehaviour
{
    public Button button;
    private ConsumableItem item;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI chargeText;
    public bool IsPlaceholder { get; private set; }

    public void Setup(ConsumableItem _item)
    {
        item = _item;
        nameText.text = item.itemName;
        button.onClick.AddListener(UseItem);
        chargeText.text = "Charges: " + item.charges;
    }

    public void SetupPlaceholder()
    {
        IsPlaceholder = true;
        item = null;
        nameText.text = "";
        chargeText.text = "";
        button.interactable = false;
    }
    private void UseItem()
    {
        if (item.Use())
            Destroy(gameObject); 
        chargeText.text = "Charges: " + item.charges;
    }
    
    public void DisableButton()
    {
        button.interactable = false;
    }
    public void EnableButton()
    {
        if (IsPlaceholder) return;
        button.interactable = true;
    }
}