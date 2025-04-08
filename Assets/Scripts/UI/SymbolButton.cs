using UnityEngine;
using UnityEngine.UI;

public class SymbolButton : MonoBehaviour
{
    private Symbol symbol;
    
    private Button button;

    public int row;
    public int col;

    public void SetUp(int col, int row)
    {
        this.row = row;
        this.col = col;
        button = GetComponent<Button>();
    }

    public void DisableButton()
    {
        button.interactable = false;
    }
    public void EnableButton()
    {
        button.interactable = true;
    }
}
