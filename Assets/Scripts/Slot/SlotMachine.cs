using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotMachine : MonoBehaviour
{
    public SlotConfig firstSlotConfig;
    public SlotConfig secondSlotConfig;

    
    private SlotConfig currentSlotConfig;

    private SlotGrid slotGrid;             

    private List<SlotReel> reels;
    
    private SymbolManager symbolManager;
    
    private SlotCalculator slotCalculator;
    private int lastWinAmount = 0; // Store last win value

    private SlotUIManager slotUIManager;

    private int currentBetAmount;
    
    
    
    private void Start()
    {
        
    }

    public void Setup(SlotCalculator slotCalculator, SlotUIManager slotUIManager, GameData gameData)
    {
        currentSlotConfig = firstSlotConfig;
        this.slotCalculator = slotCalculator;
        this.symbolManager = new SymbolManager(currentSlotConfig.symbols);
        this.slotUIManager = slotUIManager;
        slotGrid = new SlotGrid(currentSlotConfig.rows, currentSlotConfig.columns, symbolManager);
        slotUIManager.Setup(gameData, this, slotGrid);
        
        CreateReels();

    }

    private void CreateReels()
    {
        reels = new List<SlotReel>();
        for (int i = 0; i < currentSlotConfig.columns; i++)
        {
            reels.Add(new SlotReel(currentSlotConfig.symbols));
        }
    }
    
    public int SpinSlot(int betAmount, bool simulateOnly = false)
    {
        currentBetAmount = betAmount;
        for (int col = 0; col < currentSlotConfig.columns; col++)
        {
            List<Symbol> reelSymbols = reels[col].GenerateSymbols();
            for (int row = 0; row < currentSlotConfig.rows; row++)
            {
                slotGrid.SetSymbol(row, col, reelSymbols[row]);
            }
        }
        
        if (!simulateOnly)
        {
            slotUIManager.UpdateSlotUI();
        }
        
        
        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount, currentSlotConfig);

        if (!simulateOnly)
        {
            slotUIManager.DrawWinningLines(slotCalculator.winningLines, slotGrid);
            slotUIManager.AnimatePositions(slotCalculator.currentPositions, slotGrid);
        }
        
        return lastWinAmount;
    }


    //public int CalculateWin() => slotCalculator.CalculateWin(slotGrid, currentBetAmount, currentSlotConfig);
    

    

    public int RandomizeSymbolCalculate(int row, int col)
    {
        List<Symbol> possibleSymbols = new List<Symbol>(currentSlotConfig.symbols);
        possibleSymbols.Remove(slotGrid.GetSymbol(row, col));
        slotGrid.SetSymbol(row, col, possibleSymbols[Random.Range(0, possibleSymbols.Count)]);

        slotUIManager.UpdateSingleSymbol(row, col, slotGrid.GetSymbol(row, col));
        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount, currentSlotConfig, true);

        slotUIManager.DrawWinningLines(slotCalculator.winningLines, slotGrid);
        slotUIManager.AnimatePositions(slotCalculator.currentPositions, slotGrid);
        return lastWinAmount;
    }

    
}
