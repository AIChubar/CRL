using System.Collections.Generic;
using UnityEditor;

public class SlotCalculator
{
    /// <summary>
    /// Calculates the win amount based on the slot grid.
    /// </summary>
    /// <param name="slotGrid">2D array of symbols representing the slot grid.</param>
    /// <param name="betAmount">Bet amount per spin.</param>
    /// <param name="rows">Number of rows in the grid.</param>
    /// <param name="columns">Number of columns in the grid.</param>
    /// <param name="wildSymbol">The wild symbol that can substitute any symbol.</param>
    /// <param name="slotSymbols">Array of regular slot symbols.</param>
    /// <returns>Total win amount.</returns>
    public int CalculateWin(SlotGrid slotGrid, int betAmount, SlotConfig slotConfig)
    {
        int totalWin = 0;
        Dictionary<Symbol, int[]> symbolCountPerColumn = new Dictionary<Symbol, int[]>();
        Dictionary<Symbol, int[]> symbolCountPerColumnWild = new Dictionary<Symbol, int[]>();

        foreach (Symbol symbol in slotConfig.symbols)
        {
            if (symbol.isWild)
                symbolCountPerColumnWild[symbol] = new int[slotConfig.columns];
            else
                symbolCountPerColumn[symbol] = new int[slotConfig.columns];
        }

        for (int col = 0; col < slotConfig.columns; col++)
        {
            for (int row = 0; row < slotConfig.rows; row++)
            {
                Symbol symbol = slotGrid.GetSymbol(row, col);
                if (symbolCountPerColumn.ContainsKey(symbol))
                    symbolCountPerColumn[symbol][col]++;
                if (symbolCountPerColumnWild.ContainsKey(symbol))
                    symbolCountPerColumnWild[symbol][col]++;

            }
        }

        foreach (var entry in symbolCountPerColumn)
        {
            int[] counts = entry.Value;
            bool valid = true;
            int multiplier = 1;

            int nonWildAppearance = 0;
            foreach (int count in counts)
                nonWildAppearance += count;
            
            if (nonWildAppearance == 0)
                continue;

            foreach (var wildEntry in symbolCountPerColumnWild)
            {
                int[] wildCounts = wildEntry.Value;

                for (int col = 0; col < slotConfig.columns; col++)
                {
                    counts[col] += wildCounts[col];
                }
            }
            
            for (int col = 0; col < slotConfig.columns; col++)
            {
                if (counts[col] <= 0)
                {
                    valid = false;
                    break;
                }
                multiplier *= counts[col];
            }

            if (valid)
            {
                totalWin += betAmount * multiplier;
            }
        }
        foreach (var entry in symbolCountPerColumnWild)
        {
            int[] counts = entry.Value;
            bool valid = true;
            int multiplier = 1;

            for (int col = 0; col < slotConfig.columns; col++)
            {
                if (counts[col] <= 0)
                {
                    valid = false;
                    break;  
                }
                multiplier *= counts[col];
            }

            if (valid)
            {
                totalWin += betAmount * multiplier;
            }
        }
        return totalWin;
    }

    private void CalculateWinningLines()
    {
        
    }
}
