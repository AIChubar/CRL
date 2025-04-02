using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotGrid
{
    private Symbol[,] grid;
    private int rows, columns;
    private SymbolManager symbolManager;
    
    private GameObject[,] symbolInstances;
    private List<CharacterStat> columnBuffs;
    private int maxRows;

    public SlotGrid(int rows, int columns, List<CharacterStat> columnBuffs, SymbolManager symbolManager)
    {
        this.columnBuffs = columnBuffs;
        this.rows = rows;
        this.columns = columns;
        this.symbolManager = symbolManager;

        int maxBuff = columnBuffs.Count > 0 ? Mathf.Max(columnBuffs.Select(buff => buff.ValueInt).ToArray()) : 0;
        maxRows = rows + maxBuff; // Define grid height based on max buff

        grid = new Symbol[maxRows, columns];
        symbolInstances = new GameObject[maxRows, columns];

        FillGrid();
    }

    private void FillGrid()
    {
        for (int c = 0; c < columns; c++)
        {
            int activeRows = rows + columnBuffs[c].ValueInt; // How many rows are valid in this column

            for (int r = 0; r < activeRows; r++)
            {
                grid[r, c] = symbolManager.GetRandomSymbol(); // Assign valid symbols
            }
        }
    }
    public (int rows, int columns) GetRowColumnLength()
    {
        return (maxRows, columns);
    }

    public void RegisterSymbolInstance(int row, int col, GameObject instance)
    {
        if (IsValidPosition(row, col))
            symbolInstances[row, col] = instance;
    }

    public GameObject GetSymbolInstance(int row, int col)
    {
        return IsValidPosition(row, col) ? symbolInstances[row, col] : null;
    }

    public Symbol GetSymbol(int row, int column)
    {
        return IsValidPosition(row, column) ? grid[row, column] : null;
    }

    public void SetSymbol(int row, int column, Symbol newSymbol)
    {
        if (IsValidPosition(row, column))
            grid[row, column] = newSymbol;
    }

    public void SetSymbol(int row, int column, string newch)
    {
        if (IsValidPosition(row, column))
            grid[row, column].ch = newch;
    }

    public bool IsValidPosition(int row, int column)
    {
        return column >= 0 && column < columns && row >= 0 && row < (rows + columnBuffs[column].ValueInt);
    }

}
