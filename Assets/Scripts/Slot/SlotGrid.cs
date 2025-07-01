using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotGrid
{
    
    private Symbol[,] fullGrid;
    
    private List<List<Symbol>> grid;
    private List<List<SymbolButton>> symbolInstances;
    private List<CharacterStat> columnBuffs;
    private List<int> temporaryColumnBuffs;
    private SymbolManager symbolManager;

    private int rows, columns;
    
    private int totalRows = 100, totalColumns = 20;
    private int maxRowsWithBuff;


    private int gridTopLeftRow = 50;
    private int gridTopLeftColumn = 5;

    public SlotGrid(int rows, int columns, List<CharacterStat> columnBuffs, SymbolManager symbolManager)
    {
        this.columnBuffs = columnBuffs;
        this.rows = rows;
        this.columns = columns;
        this.symbolManager = symbolManager;
        temporaryColumnBuffs = new List<int>();
        for (int i = 0; i < columns; i++)
            temporaryColumnBuffs.Add(0);
        int maxBuff = columnBuffs.Count > 0 ? Mathf.Max(columnBuffs.Select(buff => buff.ValueInt).ToArray()) : 0;
        maxRowsWithBuff = rows + maxBuff;

        // Now arrays are [column, row]
        grid = new List<List<Symbol>>(columns);
        for (int c = 0; c < columns; c++)
        {
            grid.Add(new List<Symbol>(rows)); 
            for (int r = 0; r < rows + columnBuffs[c].ValueInt + temporaryColumnBuffs[c]; r++)
            {
                grid[c].Add(null);
            }
        }
        symbolInstances = new List<List<SymbolButton>>(columns);
        for (int c = 0; c < columns; c++)
        {
            symbolInstances.Add(new List<SymbolButton>(rows)); 
            for (int r = 0; r < rows + columnBuffs[c].ValueInt + temporaryColumnBuffs[c]; r++)
            {
                symbolInstances[c].Add(null);
            }
        }
        fullGrid = new Symbol[totalColumns, totalRows];
        RollFullGrid();
    }

    private void FrameGrid()
    {
        for (int c = 0; c < columns; c++)
        {
            int fullGridCol = gridTopLeftColumn + c;
            int activeRows = rows + columnBuffs[c].ValueInt + temporaryColumnBuffs[c];

            // Ensure grid[c] has enough rows
            while (grid[c].Count < activeRows)
                grid[c].Add(null);

            for (int r = 0; r < activeRows; r++)
            {
                int fullGridRow = gridTopLeftRow + r;
                grid[c][r] = fullGrid[fullGridCol, fullGridRow];
            }
        }
    }

    public void TemporaryBuffColumn(int column, int value)
    {
        temporaryColumnBuffs[column] += value;
        FrameGrid();
    }

    public void MoveGridFrame(int deltaColumns, int deltaRows)
    {
        int newCol = gridTopLeftColumn + deltaColumns;
        int newRow = gridTopLeftRow + deltaRows;

        if (newCol >= 0 && newCol + columns <= totalColumns &&
            newRow >= 0 && newRow + maxRowsWithBuff <= totalRows)
        {
            gridTopLeftColumn = newCol;
            gridTopLeftRow = newRow;
            FrameGrid(); // Refresh the visible grid
        }
        else
        {
            Debug.LogWarning("Move out of bounds: Frame not moved.");
        }
    }
    
    public Symbol GetSymbolFromFullGrid(int col, int row)
    {
        int fullCol = gridTopLeftColumn + col;
        int fullRow = gridTopLeftRow + row;

        if (fullCol >= 0 && fullCol < totalColumns && fullRow >= 0 && fullRow < totalRows)
            return fullGrid[fullCol, fullRow];

        return null;
    }
    
 

    public void RollFullGrid()
    {
        for (int c = 0; c < totalColumns; c++)
        {
            for (int r = 0; r < totalRows; r++)
            {
                fullGrid[c, r] = symbolManager.GetRandomSymbol();
            }
        }
        FrameGrid();

    }

    public (int columns, int rows) GetColumnRowLength()
    {
        return (columns, maxRowsWithBuff);
    }
    
    public (int columns, int rows) GetColumnRowLength(int col)
    {
        return (columns, rows + columnBuffs[col].ValueInt + temporaryColumnBuffs[col]);
    }

    public void RegisterSymbolInstance(int c, int r, SymbolButton instance)
    {
        if (!IsValidPosition(c, r))
            return;
        int activeRows = rows + columnBuffs[c].ValueInt + temporaryColumnBuffs[c];

        // Ensure grid[c] has enough rows
        while (symbolInstances[c].Count < activeRows)
            symbolInstances[c].Add(null);
        symbolInstances[c][r] = instance;
    }

    public SymbolButton GetSymbolInstance(int col, int row)
    {
        return IsValidPosition(col, row) ? symbolInstances[col][row] : null;
    }

    public List<List<SymbolButton>> GetSymbolButtons()
    {
        return symbolInstances;
    }

    public Symbol GetSymbol(int col, int row)
    {
        return IsValidPosition(col, row) ? grid[col][row] : null;
    }

    public void SetSymbol(int col, int row, Symbol newSymbol)
    {
        if (!IsValidPosition(col, row))
            return;

        grid[col][row] = newSymbol;

        int fullCol = gridTopLeftColumn + col;
        int fullRow = gridTopLeftRow + row;

        if (fullCol >= 0 && fullCol < totalColumns && fullRow >= 0 && fullRow < totalRows)
            fullGrid[fullCol, fullRow] = newSymbol;
    }

  

    public bool IsValidPosition(int col, int row)
    {
        return col >= 0 && col < columns && row >= 0 && row < (rows + columnBuffs[col].ValueInt + temporaryColumnBuffs[col]);
    }

    public Symbol GetFromFullGrid(int col, int row, int offset = 0)
    {
        int actualCol = gridTopLeftColumn + col;
        int actualRow = gridTopLeftRow + row + offset;

        if (actualCol >= 0 && actualCol < totalColumns && actualRow >= 0 && actualRow < totalRows)
            return fullGrid[actualCol, actualRow];
        return null;
    }
}
