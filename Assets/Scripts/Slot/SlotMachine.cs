using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotMachine 
{
    private GameData gameData;
    private SlotGrid slotGrid;             
    //private List<SlotReel> reels;
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
    }

    public List<(List<(int, int)> line, Symbol symbol)> GetWinningLines() => slotCalculator.winningLines;
    public Dictionary<Symbol, SymbolPositions> GetPlayingPositions() => slotCalculator.currentPositions;

    public int SpinSlot(int betAmount, bool simulateOnly = false)
    {
        currentBetAmount = betAmount;
      
        slotGrid.RollFullGrid();
        if (!simulateOnly)
        {
            slotUIManager.SetUpGridUI();
        }

        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount);


        return lastWinAmount;
    }
    
    

    public int RandomizeSymbolCalculate(int col, int row)
    {
        RandomizeSingleSymbol(col, row);
        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount, true);

        //slotUIManager.DrawWinningLines(slotCalculator.winningLines, slotGrid);
        //slotUIManager.AnimatePositions(slotCalculator.currentPositions, slotGrid);
        return lastWinAmount;
    }

    public int RandomizeColumnCalculate(int col)
    {
        for (int row = 0; row < slotGrid.GetColumnRowLength().rows; row++)
            RandomizeSingleSymbol(col, row);
        
        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount, true);
        //slotUIManager.DrawWinningLines(slotCalculator.winningLines, slotGrid);
        //slotUIManager.AnimatePositions(slotCalculator.currentPositions, slotGrid);
        return lastWinAmount;
    }

    private void RandomizeSingleSymbol(int col, int row)
    {
        List<Symbol> possibleSymbols = new List<Symbol>(gameData.slotConfig.symbols);
        possibleSymbols.Remove(slotGrid.GetSymbol(col, row));
        slotGrid.SetSymbol(col, row, symbolManager.GetRandomSymbolUnweighted()); //RNG
        slotUIManager.ChangeSingleSymbol(col, row, slotGrid.GetSymbol(col, row));
    }
}
