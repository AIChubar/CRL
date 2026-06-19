using UnityEngine;
using System;
using System.Collections.Generic;

public class SlotMachine
{
    private GameData gameData;
    private SlotGrid slotGrid;
    private SymbolManager symbolManager;
    private SlotCalculator slotCalculator;
    private int lastWinAmount = 0;
    private int currentBetAmount;

    /// <summary>Raised after the grid is rolled, so the view can start the spin animation.</summary>
    public event Action GridRolled;
    /// <summary>Raised after a recalculation, so the view can replay the result animation.</summary>
    public event Action ResultReady;
    /// <summary>Raised when a single symbol changes, carrying (col, row, newSymbol).</summary>
    public event Action<int, int, Symbol> SymbolChanged;

    public SlotGrid Grid => slotGrid;

    public void SetUp(SlotCalculator slotCalculator, GameData gameData)
    {
        this.gameData = gameData;
        this.slotCalculator = slotCalculator;
        this.symbolManager = new SymbolManager(gameData.slotConfig.symbols, gameData);
        slotGrid = new SlotGrid(gameData.slotConfig.rows, gameData.slotConfig.columns, gameData.columnBuffs, symbolManager);
    }

    public List<(List<(int, int)> line, Symbol symbol)> GetWinningLines() => slotCalculator.winningLines;
    public Dictionary<Symbol, SymbolPositions> GetPlayingPositions() => slotCalculator.currentPositions;

    public int SpinSlot(int betAmount)
    {
        currentBetAmount = betAmount;
        slotGrid.RollFullGrid();

        GridRolled?.Invoke();

        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount);
        return lastWinAmount;
    }

    public int RandomizeSymbolCalculate(int col, int row)
    {
        RandomizeSingleSymbol(col, row);
        return RecalculateWithAnimation();
    }

    public int RandomizeColumnCalculate(int col)
    {
        for (int row = 0; row < slotGrid.GetColumnRowLength().rows; row++)
            RandomizeSingleSymbol(col, row, true);
        return RecalculateWithAnimation();
    }

    public int RecalculateWithAnimation()
    {
        lastWinAmount = slotCalculator.CalculateWin(slotGrid, currentBetAmount, true);
        ResultReady?.Invoke();
        return lastWinAmount;
    }

    private void RandomizeSingleSymbol(int col, int row, bool canBeTheSame = false)
    {
        slotGrid.SetSymbol(col, row, symbolManager.GetRandomSymbolUnweighted(canBeTheSame ? null : slotGrid.GetSymbol(col, row), true));
        SymbolChanged?.Invoke(col, row, slotGrid.GetSymbol(col, row));
    }
}
