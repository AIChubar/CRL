using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotMachine : MonoBehaviour
{
    public SlotConfig firstSlotConfig;
    public SlotConfig secondSlotConfig;

    
    private SlotConfig currentSlotConfig;

    private string[,] slotGrid;             

    private List<SlotReel> reels;
    
    private SlotCalculator slotCalculator;
    private int lastWinAmount = 0; // Store last win value

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

    public void Setup(SlotCalculator slotCalculator)
    {
        this.slotCalculator = slotCalculator;
    }

    public int SpinSlot(int betAmount, bool simulateOnly = false)
    {
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
            GameManager.instance.slotUIManager.UpdateSlotUI(slotGrid);
        }
        lastWinAmount = slotCalculator.CalculateWin(slotGrid, betAmount, currentSlotConfig.rows, currentSlotConfig.columns, currentSlotConfig.wildSymbol, currentSlotConfig.slotSymbols);

        return lastWinAmount;
    }


    public int GetLastWin()
    {
        return lastWinAmount;
    }

    

    public void RandomizeSymbol(int row, int col)
    {
        List<string> possibleSymbols = new List<string>(currentSlotConfig.slotSymbols) { currentSlotConfig.wildSymbol };
        possibleSymbols.Remove(slotGrid[row, col]);
        slotGrid[row, col] = possibleSymbols[Random.Range(0, possibleSymbols.Count)];

        GameManager.instance.slotUIManager.UpdateSingleSymbol(row, col, slotGrid[row, col]);
        
    }

    
}
