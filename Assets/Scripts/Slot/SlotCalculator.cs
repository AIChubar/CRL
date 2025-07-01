#nullable enable
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;

public class SlotCalculator
{
    
public List<(List<(int, int)> line, Symbol symbol)> winningLines = new List<(List<(int, int)>, Symbol)>();
private Dictionary<Symbol, SymbolPositions> playingPositions = new Dictionary<Symbol, SymbolPositions>();
public Dictionary<Symbol, SymbolPositions> currentPositions = new Dictionary<Symbol, SymbolPositions>();

public int CalculateWin(SlotGrid slotGrid, int betAmount, bool isRecalculating = false)
{
    int totalWin = 0;
    winningLines = new List<(List<(int, int)>, Symbol)>();
    playingPositions = new Dictionary<Symbol, SymbolPositions>();

    for (int startRow = 0; startRow < slotGrid.GetColumnRowLength(0).rows; startRow++)
    {
        if (!slotGrid.IsValidPosition(0, startRow))
        {
            continue;
        }

        List<List<(int, int)>> possibleWins = new List<List<(int, int)>> { new List<(int, int)> { (0, startRow) } };

        for (int col = 1; col < slotGrid.GetColumnRowLength().columns; col++)
        {
            List<List<(int, int)>> newPossibleWins = new List<List<(int, int)>>();

            for (int row = 0; row < slotGrid.GetColumnRowLength(col).rows; row++)
            {
                if (!slotGrid.IsValidPosition(col, row))
                {
                    continue;
                }

                Symbol currentSymbol = slotGrid.GetSymbol(col, row);

                foreach (var winPath in possibleWins)
                {
                    Symbol? assignedSymbol = DetermineAssignedSymbol(winPath, slotGrid);

                    if (currentSymbol == assignedSymbol || currentSymbol.isWild || assignedSymbol == null)
                    {
                        List<(int, int)> newWinPath = new List<(int, int)>(winPath) { (col, row) };
                        newPossibleWins.Add(newWinPath);
                    }
                }
            }

            if (newPossibleWins.Count == 0)
            {
                possibleWins.Clear();
                break;
            }

            possibleWins = newPossibleWins;
        }

        foreach (var winPath in possibleWins)
        {
            Symbol winningSymbol = DetermineAssignedSymbol(winPath, slotGrid) ?? slotGrid.GetSymbol(winPath[0].Item1, winPath[0].Item2);
            winningLines.Add((winPath, winningSymbol));

            if (!playingPositions.ContainsKey(winningSymbol))
            {
                playingPositions[winningSymbol] = new SymbolPositions(new List<(int, int)>());
            }

            foreach (var pos in winPath)
            {
                if (!playingPositions[winningSymbol].Positions.Contains(pos))
                {
                    playingPositions[winningSymbol].Positions.Add(pos);
                }
            }

            totalWin += betAmount; // Adjust based on symbol payouts
        }
    }

    // Now compare with previous positions AFTER creating playingPositions
    if (isRecalculating)
    {
        foreach (var symbol in playingPositions.Keys)
        {
            if (currentPositions.TryGetValue(symbol, out var prev))
            {
                // Reset WasShown to false if the new count of positions is greater
                if (playingPositions[symbol].Positions.Count > prev.Positions.Count)
                {
                    playingPositions[symbol].WasShown = false;
                }
                else
                {
                    playingPositions[symbol].WasShown = prev.WasShown;
                }
            }
        }
    }

    currentPositions = playingPositions;
    return totalWin;
}

private Symbol? DetermineAssignedSymbol(List<(int, int)> winPath, SlotGrid slotGrid)
{
    Symbol? assignedSymbol = null;
    foreach (var (col, row) in winPath)
    {
        Symbol symbol = slotGrid.GetSymbol(col, row);
        if (!symbol.isWild)
        {
            if (assignedSymbol == null)
            {
                assignedSymbol = symbol;
            }
            else if (symbol != assignedSymbol)
            {
                return null; // Invalid path: multiple non-matching symbols
            }
        }
    }
    return assignedSymbol;
}
}

public class SymbolPositions
{
    public List<(int, int)> Positions { get; }
    public bool WasShown { get; set; } // Defaults to false


    public SymbolPositions(List<(int, int)> positions)
    {
        Positions = positions;
        WasShown = false;
    }
}