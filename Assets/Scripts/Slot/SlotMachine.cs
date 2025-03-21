using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotMachine : MonoBehaviour
{
    public SlotConfig firstSlotConfig;
    public SlotConfig secondSlotConfig;

    
    private SlotConfig currentSlotConfig;
    public GameObject slotPanel;            
    public GameObject slotSymbolPrefab;     

    private string[,] slotGrid;             

    private List<SlotReel> reels;
    
    private void Start()
    {
        currentSlotConfig = firstSlotConfig;
        reels = new List<SlotReel>();
        for (int i = 0; i < currentSlotConfig.columns; i++)
        {
            reels.Add(new SlotReel(currentSlotConfig.rows, currentSlotConfig.wildChance, currentSlotConfig.wildSymbol, currentSlotConfig.slotSymbols));
        }
        slotGrid = new string[currentSlotConfig.rows, currentSlotConfig.columns];
    }

    public int SpinSlot(int betAmount, bool simulateOnly = false)
{
    if (!simulateOnly)
    {
        foreach (Transform child in slotPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    for (int col = 0; col < currentSlotConfig.columns; col++)
    {
        string[] reelSymbols = reels[col].GenerateSymbols();
        for (int row = 0; row < currentSlotConfig.rows; row++)
        {
            slotGrid[row, col] = reelSymbols[row];
        }
    }

    if (!simulateOnly)
    {
        GridLayoutGroup grid = slotPanel.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            grid = slotPanel.AddComponent<GridLayoutGroup>();
        }

        float panelWidth = 1200f;
        float panelHeight = 750f;

        float maxCellWidth = panelWidth / currentSlotConfig.columns;
        float maxCellHeight = panelHeight / currentSlotConfig.rows;

        float spacingX = maxCellWidth / 10f;
        float spacingY = maxCellHeight / 10f;

        float cellWidth = maxCellWidth - spacingX;
        float cellHeight = maxCellHeight - spacingY;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.spacing = new Vector2(spacingX, spacingY);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = currentSlotConfig.columns;
        grid.childAlignment = TextAnchor.MiddleCenter;

        for (int row = 0; row < currentSlotConfig.rows; row++)
        {
            for (int col = 0; col < currentSlotConfig.columns; col++)
            {
                GameManager.instance.slotUIManager.CreateSymbolUI(slotSymbolPrefab, slotPanel, slotGrid, row, col,
                    cellWidth, cellHeight);
            }
        }
    }

    return SlotCalculator.CalculateWin(slotGrid, betAmount, currentSlotConfig.rows, currentSlotConfig.columns, currentSlotConfig.wildSymbol, currentSlotConfig.slotSymbols);
}



    
    public int CalculateWin(int betAmount)
    {
        return SlotCalculator.CalculateWin(slotGrid, betAmount, currentSlotConfig.rows, currentSlotConfig.columns, currentSlotConfig.wildSymbol, currentSlotConfig.slotSymbols);
        
    }

    public void RandomizeSymbol(int row, int col)
    {
        List<string> possibleSymbols = new List<string>(currentSlotConfig.slotSymbols) { currentSlotConfig.wildSymbol };
        possibleSymbols.Remove(slotGrid[row, col]);
        slotGrid[row, col] = possibleSymbols[Random.Range(0, possibleSymbols.Count)];

        Transform symbolTransform = slotPanel.transform.GetChild(row * currentSlotConfig.columns + col);
        symbolTransform.GetComponent<TMP_Text>().text = slotGrid[row, col];
    }

    
}
