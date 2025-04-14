using System;
using System.Collections;
using UnityEngine;

public class SlotSpinAnimator //vibe coded
{
    private SlotGrid slotGrid;
    // Total time of the spin animation for each column.
    private float spinDuration;
    // Delay between starting each column's animation.
    private float delayBetweenColumns;
    // Spacing between symbols (assumed to be the height in pixels of a symbol).
    private float spacing = 100f;
    // Fixed distance (in symbol units) that the reel should travel.
    private int fixedSymbolOffset = 10;

    private CoroutineTracker coroutineTracker;

    public SlotSpinAnimator(SlotGrid slotGrid, MonoBehaviour coroutineRunner,
                            float spinDuration = 3.5f,
                            float delayBetweenColumns = 0.3f)
    {
        this.slotGrid = slotGrid;
        this.spinDuration = spinDuration;
        this.delayBetweenColumns = delayBetweenColumns;
        coroutineTracker = new CoroutineTracker(coroutineRunner, AnimationType.Spin);
    }

    /// <summary>
    /// Begins the spin animation for the entire grid.
    /// </summary>
    public void StartSpin()
    {
        coroutineTracker.StartTrackedCoroutine(SpinGrid());
    }

    /// <summary>
    /// Spins every column (reel) in a staggered fashion.
    /// </summary>
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
        // Optionally add a brief pause after all columns finish.
        yield return new WaitForSeconds(0.3f);
    }

    /// <summary>
    /// Animates a single reel column by moving its symbol buttons to simulate rolling.
    /// The reel starts with a fixed offset of 20 symbols (taken from the full grid)
    /// and gradually “rolls” to reveal the final symbols from the SlotGrid.
    /// </summary>
    /// <param name="col">Index of the column.</param>
    /// <param name="onColumnComplete">Callback invoked when the column finishes its animation.</param>
    private IEnumerator SpinSingleColumn(int col, Action onColumnComplete)
    {
        // Get the number of active rows for this column.
        int rows = slotGrid.GetColumnRowLength().rows;
        // Cache references to SymbolButton instances and their starting positions.
        SymbolButton[] buttons = new SymbolButton[rows];
        Vector3[] startPositions = new Vector3[rows];
        // Cache the final (framed) symbols for the column.
        Symbol[] finalColumnSymbols = new Symbol[rows];

        for (int row = 0; row < rows; row++)
        {
            buttons[row] = slotGrid.GetSymbolInstance(col, row);
            if (buttons[row] != null)
            {
                // Record the final (target) local position.
                startPositions[row] = buttons[row].transform.localPosition;
            }
            finalColumnSymbols[row] = slotGrid.GetSymbol(col, row);
        }

        // The animation will interpolate a current offset value from fixedSymbolOffset to 0.
        float t = 0f;
        while (t < spinDuration)
        {
            float progress = t / spinDuration;
            // Apply easing to progress for a more natural slowdown
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            float currentOffset = Mathf.Lerp(fixedSymbolOffset, 0, easedProgress);
            int offsetInt = Mathf.FloorToInt(currentOffset);
            float fractional = currentOffset - offsetInt;
            float shift = fractional * spacing;

            // Update every row in the column.
            for (int row = 0; row < rows; row++)
            {
                Symbol symbolToShow;
                if (offsetInt > 0)
                {
                    symbolToShow = slotGrid.GetFromFullGrid(col, row, offsetInt);
                }
                else
                {
                    symbolToShow = finalColumnSymbols[row];
                }
        
                if (buttons[row] != null)
                {
                    // Set the symbol image
                    buttons[row].SetSymbol(symbolToShow);
                    // Calculate the target position based on the starting position and the shift.
                    Vector3 targetPosition = startPositions[row] - new Vector3(0, shift, 0);
                    // Smoothly interpolate the button's position. Adjust smoothingFactor as needed.
                    float smoothingFactor = 15f;
                    buttons[row].transform.localPosition = Vector3.Lerp(buttons[row].transform.localPosition, targetPosition, Time.deltaTime * smoothingFactor);
                }
            }
    
            t += Time.deltaTime;
            yield return null;
        }



        // Ensure final state: all buttons at their starting positions displaying final symbols.
        for (int row = 0; row < rows; row++)
        {
            if (buttons[row] != null)
            {
                buttons[row]?.SetSymbol(finalColumnSymbols[row]);
                buttons[row].transform.localPosition = startPositions[row];
            }
        }

        // Mark this column as finished.
        onColumnComplete?.Invoke();
    }
}
