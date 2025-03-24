using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;

public class SlotCalculator
{

    public List<List<(int, int)>> winningLines =  new List<List<(int, int)>>();
    public int CalculateWin(SlotGrid slotGrid, int betAmount, SlotConfig slotConfig)
    {
        int totalWin = 0;
        winningLines = new List<List<(int, int)>>();

        // Iterate through each row in the first column
        for (int startRow = 0; startRow < slotConfig.rows; startRow++)
        {
            Symbol targetSymbol = slotGrid.GetSymbol(startRow, 0);

            // Store all possible winning paths starting from this symbol
            List<List<(int, int)>> possibleWins = new List<List<(int, int)>> { new List<(int, int)> { (startRow, 0) } };

            // Iterate through the remaining columns
            for (int col = 1; col < slotConfig.columns; col++)
            {
                List<List<(int, int)>> newPossibleWins = new List<List<(int, int)>>();

                for (int row = 0; row < slotConfig.rows; row++)
                {
                    if (slotGrid.GetSymbol(row, col) == targetSymbol)
                    {
                        // Extend each current winning path with the new symbol found
                        foreach (var winPath in possibleWins)
                        {
                            List<(int, int)> newWinPath = new List<(int, int)>(winPath) { (row, col) };
                            newPossibleWins.Add(newWinPath);
                        }
                    }
                }

                // If no matches in this column, stop checking further
                if (newPossibleWins.Count == 0)
                    break;

                possibleWins = newPossibleWins;
            }

            // Add completed winning lines
            winningLines.AddRange(possibleWins);
        }

        // Calculate the total win based on the number of winning lines
        foreach (var line in winningLines)
        {
            totalWin += betAmount * line.Count;
        }

        return totalWin;
    }
    
}
