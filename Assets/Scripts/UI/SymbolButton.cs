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
    private TextMeshProUGUI symbolText;
    public void SetUp(int col, int row)
    {
        symbolText = GetComponent<TextMeshProUGUI>();
        this.row = row;
        this.col = col;
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetSymbol(Symbol symbol)
    {
        this.symbol = symbol;
        symbolText.text = symbol.ch;
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
