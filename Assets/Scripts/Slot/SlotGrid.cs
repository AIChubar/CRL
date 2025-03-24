using System;

public class SlotGrid
{
    private Symbol[,] grid;
    private int rows, columns;
    private SymbolManager symbolManager;

    public SlotGrid(int rows, int columns, SymbolManager symbolManager)
    {
        this.rows = rows;
        this.columns = columns;
        this.symbolManager = symbolManager;
        grid = new Symbol[rows, columns];
        FillGrid();
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