using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;

public class SlotCalculator
{
    public List<List<(int, int)>> winningLines = new List<List<(int, int)>>();

    public int CalculateWin(SlotGrid slotGrid, int betAmount, SlotConfig slotConfig)
    {
        int totalWin = 0;
        winningLines = new List<List<(int, int)>>();

        for (int startRow = 0; startRow < slotConfig.rows; startRow++)
        {
            List<List<(int, int)>> possibleWins = new List<List<(int, int)>> { new List<(int, int)> { (startRow, 0) } };

            for (int col = 1; col < slotConfig.columns; col++)
            {
                List<List<(int, int)>> newPossibleWins = new List<List<(int, int)>>();

                for (int row = 0; row < slotConfig.rows; row++)
                {
                    Symbol currentSymbol = slotGrid.GetSymbol(row, col);

                    foreach (var winPath in possibleWins)
                    {
                        Symbol? assignedSymbol = DetermineAssignedSymbol(winPath, slotGrid);

                        // A valid extension: same symbol or wild (wild adapts)
                        if (currentSymbol == assignedSymbol || currentSymbol.isWild || assignedSymbol == null)
                        {
                            List<(int, int)> newWinPath = new List<(int, int)>(winPath) { (row, col) };
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

            winningLines.AddRange(possibleWins);
        }

        foreach (var line in winningLines)
        {
            totalWin += betAmount;
        }

        return totalWin;
    }

    private Symbol? DetermineAssignedSymbol(List<(int, int)> winPath, SlotGrid slotGrid)
    {
        Symbol? assignedSymbol = null;
        foreach (var (row, col) in winPath)
        {
            Symbol symbol = slotGrid.GetSymbol(row, col);
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
