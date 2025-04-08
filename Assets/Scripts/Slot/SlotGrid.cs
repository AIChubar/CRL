using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotGrid
{
    private Symbol[,] grid;
    private SymbolButton[,] symbolInstances;
    private List<CharacterStat> columnBuffs;
    private SymbolManager symbolManager;

    private int rows, columns;
    private int maxRows;

    public SlotGrid(int rows, int columns, List<CharacterStat> columnBuffs, SymbolManager symbolManager)
    {
        this.columnBuffs = columnBuffs;
        this.rows = rows;
        this.columns = columns;
        this.symbolManager = symbolManager;

        int maxBuff = columnBuffs.Count > 0 ? Mathf.Max(columnBuffs.Select(buff => buff.ValueInt).ToArray()) : 0;
        maxRows = rows + maxBuff;

        // Now arrays are [column, row]
        grid = new Symbol[columns, maxRows];
        symbolInstances = new SymbolButton[columns, maxRows];

        FillGrid();
    }

    private void FillGrid()
    {
        for (int c = 0; c < columns; c++)
        {
            int activeRows = rows + columnBuffs[c].ValueInt;

            for (int r = 0; r < activeRows; r++)
            {
                grid[c, r] = symbolManager.GetRandomSymbol();
            }
        }
    }

    public (int columns, int rows) GetColumnRowLength()
    {
        return (columns, maxRows);
    }

    public void RegisterSymbolInstance(int col, int row, SymbolButton instance)
    {
        if (IsValidPosition(col, row))
            symbolInstances[col, row] = instance;
    }

    public SymbolButton GetSymbolInstance(int col, int row)
    {
        return IsValidPosition(col, row) ? symbolInstances[col, row] : null;
    }

    public SymbolButton[,] GetSymbolButtons()
    {
        return symbolInstances;
    }

    public Symbol GetSymbol(int col, int row)
    {
        return IsValidPosition(col, row) ? grid[col, row] : null;
    }

    public void SetSymbol(int col, int row, Symbol newSymbol)
    {
        if (IsValidPosition(col, row))
            grid[col, row] = newSymbol;
    }

    public void SetSymbol(int col, int row, string newch)
    {
        if (IsValidPosition(col, row))
            grid[col, row].ch = newch;
    }

    public bool IsValidPosition(int col, int row)
    {
        return col >= 0 && col < columns && row >= 0 && row < (rows + columnBuffs[col].ValueInt);
    }
}
