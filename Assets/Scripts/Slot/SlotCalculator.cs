using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;

public class SlotCalculator
{
    public List<(List<(int, int)> line, Symbol symbol)> winningLines = new List<(List<(int, int)>, Symbol)>();
    public Dictionary<Symbol, List<(int, int)>> playingPositions = new Dictionary<Symbol, List<(int, int)>>();

    public int CalculateWin(SlotGrid slotGrid, int betAmount, SlotConfig slotConfig)
    {
        int totalWin = 0;
        winningLines = new List<(List<(int, int)>, Symbol)>();
        playingPositions = new Dictionary<Symbol, List<(int, int)>>();
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

            foreach (var winPath in possibleWins)
            {
                Symbol winningSymbol = DetermineAssignedSymbol(winPath, slotGrid) ?? slotGrid.GetSymbol(winPath[0].Item1, winPath[0].Item2);
                winningLines.Add((winPath, winningSymbol));

                if (!playingPositions.ContainsKey(winningSymbol))
                {
                    playingPositions[winningSymbol] = new List<(int, int)>();
                }
                playingPositions[winningSymbol].AddRange(winPath.Where(pos => !playingPositions[winningSymbol].Contains(pos)));

                totalWin += betAmount; // Adjust based on symbol payouts
            }
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
