using System;
using System.Collections;
using UnityEngine;

public class SlotSpinAnimator
{
    private SlotGrid slotGrid;
    private float spinDuration;
    private float delayBetweenColumns;
    private float symbolScrollSpeed;
    private CoroutineTracker coroutineTracker;

    public SlotSpinAnimator(SlotGrid slotGrid, MonoBehaviour coroutineRunner, float spinDuration = 1.5f, float delayBetweenColumns = 0.3f, float symbolScrollSpeed = 0.05f)
    {
        this.slotGrid = slotGrid;
        this.spinDuration = spinDuration;
        this.delayBetweenColumns = delayBetweenColumns;
        this.symbolScrollSpeed = symbolScrollSpeed;
        coroutineTracker = new CoroutineTracker(coroutineRunner, AnimationType.Spin);
    }

    public void StartSpin()
    {
        coroutineTracker.StartTrackedCoroutine(SpinGrid());
    }
    

    public IEnumerator SpinGrid()
    {
        var (columns, _) = slotGrid.GetColumnRowLength();
        int columnsCount = columns;
        int columnsFinished = 0;

        // Start each column's spin with a staggered delay.
        for (int col = 0; col < columnsCount; col++)
        {
            coroutineTracker.StartTrackedCoroutine(SpinSingleColumn(col, () =>
            {
                columnsFinished++;
            }));
            yield return new WaitForSeconds(delayBetweenColumns);
        }

        // Wait until all columns have finished.
        while (columnsFinished < columnsCount)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator SpinSingleColumn(int col, Action onColumnComplete)
    {
        float elapsed = 0f;
        int rows = slotGrid.GetColumnRowLength().rows;  // Ensure that SlotGrid has a method to get max rows for a column.
        Symbol[] finalColumnSymbols = new Symbol[rows];

        // Cache final symbols from the SlotGrid.
        for (int row = 0; row < rows; row++)
        {
            finalColumnSymbols[row] = slotGrid.GetSymbol(col, row);
        }

        // Animate the spin for the column.
        while (elapsed < spinDuration)
        {
            for (int row = 0; row < rows; row++)
            {
                SymbolButton button = slotGrid.GetSymbolInstance(col, row);
                if (button != null)
                {
                    // Use a fake offset (optionally adjust logic to use a proper sequential offset)
                    int offset = UnityEngine.Random.Range(1, 10);
                    Symbol fakeSymbol = slotGrid.GetFromFullGrid(col, row, offset);
                    button.SetSymbol(fakeSymbol);
                }
            }

            yield return new WaitForSeconds(symbolScrollSpeed);
            elapsed += symbolScrollSpeed;
        }

        // Spin finished: display the actual symbols for this column.
        for (int row = 0; row < rows; row++)
        {
            SymbolButton button = slotGrid.GetSymbolInstance(col, row);
            button?.SetSymbol(finalColumnSymbols[row]);
        }

        // Mark this column as done.
        onColumnComplete?.Invoke();
    }
}
