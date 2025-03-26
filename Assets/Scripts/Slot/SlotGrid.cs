using System;
using UnityEngine;

public class SlotGrid
{
    private Symbol[,] grid;
    private int rows, columns;
    private SymbolManager symbolManager;
    
    private GameObject[,] symbolInstances;

    public SlotGrid(int rows, int columns, SymbolManager symbolManager)
    {
        this.rows = rows;
        this.columns = columns;
        this.symbolManager = symbolManager;
        grid = new Symbol[rows, columns];
        symbolInstances = new GameObject[rows, columns];
        FillGrid();
    }

    public void RegisterSymbolInstance(int row, int col, GameObject instance)
    {
        symbolInstances[row, col] = instance;
    }

    public GameObject GetSymbolInstance(int row, int col)
    {
        return symbolInstances[row, col];
    }
    
    private void FillGrid()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                grid[r, c] = symbolManager.GetRandomSymbol();
            }
        }
    }

    public (int rows, int columns) GetRowColumnLength()
    {
        return (rows, columns );
    }

    public void SetGridTest(SlotGridTestCase predefinedGrid)
    {
        for (int r = 0; r < predefinedGrid.grid.Count; r++)
        {
            for (int c = 0; c < predefinedGrid.grid[r].rowValues.Count; c++)
            {
                grid[r, c] = symbolManager.GetSymbolByIndex(predefinedGrid.grid[r].rowValues[c]);
            }
        }
    }

    public Symbol GetSymbol(int row, int column)
    {
        return grid[row, column];
    }

    public void SetSymbol(int row, int column, Symbol newSymbol)
    {
        grid[row, column] = newSymbol;
    }
    
    public void SetSymbol(int row, int column, string newch)
    {
        grid[row, column].ch = newch;
    }
}