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
        for (int col = 0; col < slotGrid.GetColumnRowLength().columns; col++)
        {
            List<Symbol> reelSymbols = reels[col].GenerateSymbols(slotGrid.GetColumnRowLength().rows * 10);
            for (int row = 0; row < slotGrid.GetColumnRowLength().rows; row++)
            {
                slotGrid.SetSymbol(col, row, reelSymbols[row + slotGrid.GetColumnRowLength().rows * 8]);
            }
        }

        if (!simulateOnly)
        {
            slotUIManager.SetUpGridUI();
        }

        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount);

        if (!simulateOnly)
        {
            slotUIManager.DrawWinningLines(slotCalculator.winningLines, slotGrid);
            slotUIManager.AnimatePositions(slotCalculator.currentPositions, slotGrid);
        }

        return lastWinAmount;
    }

    public int RandomizeSymbolCalculate(int col, int row)
    {
        List<Symbol> possibleSymbols = new List<Symbol>(gameData.slotConfig.symbols);
        possibleSymbols.Remove(slotGrid.GetSymbol(col, row));
        slotGrid.SetSymbol(col, row, possibleSymbols[Random.Range(0, possibleSymbols.Count)]);

        slotUIManager.ReRollSingleSymbol(col, row, slotGrid.GetSymbol(col, row));
        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount, true);

        slotUIManager.DrawWinningLines(slotCalculator.winningLines, slotGrid);
        slotUIManager.AnimatePositions(slotCalculator.currentPositions, slotGrid);
        return lastWinAmount;
    }
}
