using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableItemButton : MonoBehaviour
{
    public Button button;
    private ConsumableItem item;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI chargeText;
    public void Setup(ConsumableItem _item)
    {
        item = _item;
        nameText.text = item.itemName;
        button.onClick.AddListener(UseItem);
        chargeText.text = "Charges: " + item.charges;
    }
    private void UseItem()
    {
        if (item.Use())
            Destroy(gameObject); 
        chargeText.text = "Charges: " + item.charges;
    }
}