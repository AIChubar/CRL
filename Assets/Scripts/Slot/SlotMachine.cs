using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotMachine 
{

    
    private GameData gameData;

    private SlotGrid slotGrid;             

    private List<SlotReel> reels;
    
    private SymbolManager symbolManager;
    
    private SlotCalculator slotCalculator;
    private int lastWinAmount = 0; // Store last win value

    private SlotUIManager slotUIManager;

    private int currentBetAmount;
    
    
    
    public void SetUp(SlotCalculator slotCalculator, SlotUIManager slotUIManager, GameData gameData)
    {
        this.gameData = gameData;
        this.slotCalculator = slotCalculator;
        this.symbolManager = new SymbolManager(gameData.slotConfig.symbols);
        this.slotUIManager = slotUIManager;
        slotGrid = new SlotGrid(gameData.slotConfig.rows, gameData.slotConfig.columns, gameData.columnBuffs, symbolManager);
        slotUIManager.SetUp(gameData, this, slotGrid);
        
        CreateReels();

    }

    private void CreateReels()
    {
        reels = new List<SlotReel>();
        for (int i = 0; i < gameData.slotConfig.columns; i++)
        {
            reels.Add(new SlotReel(gameData.slotConfig.symbols));
        }
    }
    
    public int SpinSlot(int betAmount, bool simulateOnly = false)
    {
        currentBetAmount = betAmount;
        for (int col = 0; col < slotGrid.GetRowColumnLength().columns; col++)
        {
            List<Symbol> reelSymbols = reels[col].GenerateSymbols(slotGrid.GetRowColumnLength().rows * 10);
            for (int row = 0; row < slotGrid.GetRowColumnLength().rows; row++)
            {
                slotGrid.SetSymbol(row, col, reelSymbols[row + slotGrid.GetRowColumnLength().rows*8]);
            }
        }
        
        if (!simulateOnly)
        {
            slotUIManager.SetUpGrid();
        }
        
        
        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount);

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
        List<Symbol> possibleSymbols = new List<Symbol>(gameData.slotConfig.symbols);
        possibleSymbols.Remove(slotGrid.GetSymbol(row, col));
        slotGrid.SetSymbol(row, col, possibleSymbols[Random.Range(0, possibleSymbols.Count)]);

        slotUIManager.UpdateSingleSymbol(row, col, slotGrid.GetSymbol(row, col));
        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount,  true);

        slotUIManager.DrawWinningLines(slotCalculator.winningLines, slotGrid);
        slotUIManager.AnimatePositions(slotCalculator.currentPositions, slotGrid);
        return lastWinAmount;
    }

    
}
