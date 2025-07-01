using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class SlotSpinAnimator
{
    private SlotGrid slotGrid;
    private float symbolStepDuration = 0.03f;
    private float delayBetweenColumns = 0.1f;
    private int symbolSteps = 30;
    private GameData gameData;
    private CoroutineTracker coroutineTracker;

    public SlotSpinAnimator(SlotGrid slotGrid, MonoBehaviour coroutineRunner, GameData gameData)
    {
        this.gameData = gameData;
        this.slotGrid = slotGrid;
        this.coroutineTracker = new CoroutineTracker(coroutineRunner, AnimationType.Spin);
    }

    public void StartSpin()
    {
        coroutineTracker.StartTrackedCoroutine(SpinGrid());
    }

    private IEnumerator SpinGrid()
    {
        var (columns, _) = slotGrid.GetColumnRowLength();
        int columnsCount = columns;
        int columnsFinished = 0;

        for (int col = 0; col < columnsCount; col++)
        {
            coroutineTracker.StartTrackedCoroutine(SpinSingleColumn(col, () =>
            {
                columnsFinished++;
            }));

            yield return new WaitForSeconds(delayBetweenColumns * gameData.animationSpeed);
        }

        while (columnsFinished < columnsCount)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator SpinSingleColumn(int col, Action onColumnComplete)
    {
        int rows = slotGrid.GetColumnRowLength(col).rows;
        SymbolButton[] originals = new SymbolButton[rows];
        List<SymbolButton> copies = new List<SymbolButton>();
        Vector3[] positions = new Vector3[rows+1];
        
        List<Symbol> symbolsToShow = new List<Symbol>();

        
        // 1. Copy each symbol, hide the original
        for (int row = 0; row < rows; row++) // start here, symbols are ok
        {
            var original = slotGrid.GetSymbolInstance(col, row);
            originals[row] = original;
            positions[row+1] = original.transform.localPosition;

            // Create a copy
            GameObject copyObj = GameObject.Instantiate(original.gameObject, original.transform.parent);
            
            copyObj.transform.localPosition = positions[row+1];
            SymbolButton copy = copyObj.GetComponent<SymbolButton>();
            copies.Add(copy);

            // Optionally distinguish the copy visually (e.g. faded, different material)
            original.gameObject.SetActive(false);
            copy.gameObject.SetActive(true);
        }
        
        
        for (int row =  symbolSteps -1  ; row >=0 ; row--)
        {
            symbolsToShow.Add(slotGrid.GetSymbolFromFullGrid(col, row ));
        }
        float spacing = positions[1].y - positions[2].y;
        
        
        
        GameObject additional = GameObject.Instantiate(slotGrid.GetSymbolInstance(col, 0).gameObject, slotGrid.GetSymbolInstance(col, 0).transform.parent);
        
        additional.SetActive(true);
        SymbolButton additionalSymbolButton = additional.GetComponent<SymbolButton>();
        copies.Add(additionalSymbolButton);
        positions[0] = positions[1] + new Vector3(0,spacing,0);;
        
        // 2. Animate the copies
        for (int step = 0; step < symbolSteps; step++)
        {
            float elapsed = 0f;
            SymbolButton symbolButtonToMove = copies[^1];
            copies.RemoveAt(copies.Count - 1);
            symbolButtonToMove.SetSymbol(symbolsToShow[step]);
            symbolButtonToMove.transform.localPosition = positions[0];
            copies.Insert(0, symbolButtonToMove);
            while (elapsed < symbolStepDuration  * gameData.animationSpeed)
            {
                float t = Mathf.Clamp01(elapsed / symbolStepDuration / gameData.animationSpeed);
                float offset = Mathf.Lerp(0, spacing, t);

                for (int i = 0; i < rows+1; i++)
                {
                    copies[i].transform.localPosition = positions[i] - new Vector3(0f, offset, 0f);
                }

                yield return null;
                elapsed += Time.deltaTime;

            }
            
            
        }

        // 3. Replace final symbols and restore originals
        for (int row = 0; row < rows; row++)
        {
            SymbolButton original = originals[row];

            // Set correct final position and symbol on the original
            //original.transform.localPosition = copy.transform.localPosition;
            original.SetSymbol(slotGrid.GetSymbol(col, row));

            // Hide and destroy the copy, show original
            original.gameObject.SetActive(true);
            
        }

        foreach (var copy in copies)
        {
            GameObject.Destroy(copy.gameObject);
        }
        copies.Clear();
        onColumnComplete?.Invoke();
    }


    private float GetHighestY(SymbolButton[] buttons, Vector3[] referencePositions)
    {
        float highest = float.MinValue;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                float y = buttons[i].transform.localPosition.y;
                if (y > highest) highest = y;
            }
        }
        return highest;
    }
}

