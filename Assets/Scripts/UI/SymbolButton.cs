using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SymbolButton : MonoBehaviour
{
    private Symbol symbol;
    
    private Button button;
    private CanvasGroup canvasGroup;
    public int row;
    public int col;
    //public TextMeshProUGUI symbolText;
    public Image symbolImage;
    public void SetUp(int col, int row)
    {
        this.row = row;
        this.col = col;
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetSymbol(Symbol symbol)
    {
        this.symbol = symbol;
        this.symbolImage.sprite = symbol.sprite;
        //symbolText.text = symbol.ch;
        
    }
    
    public void DisableButton()
    {
        button.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public void EnableButton()
    {
        button.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
