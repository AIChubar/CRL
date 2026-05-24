using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SlotSpinAnimator
{
    private SlotGrid slotGrid;
    private float symbolStepDuration = 0.03f;
    private float delayBetweenColumns = 0.1f;
    private int symbolSteps = 30;
    private GameData gameData;

    public SlotSpinAnimator(SlotGrid slotGrid, GameData gameData)
    {
        this.gameData = gameData;
        this.slotGrid = slotGrid;
    }

    public async Awaitable StartSpin(CancellationToken ct)
    {
        var (columns, _) = slotGrid.GetColumnRowLength();
        var tasks = new List<Awaitable>();

        for (int col = 0; col < columns; col++)
        {
            tasks.Add(SpinSingleColumn(col, ct));
            await Awaitable.WaitForSecondsAsync(delayBetweenColumns * gameData.animationSpeed, ct);
        }

        foreach (var task in tasks)
            await task;
        await Awaitable.WaitForSecondsAsync(0.2f, ct);
    }

    private async Awaitable SpinSingleColumn(int col, CancellationToken ct)
    {
        int rows = slotGrid.GetColumnRowLength(col).rows;
        SymbolButton[] originals = new SymbolButton[rows];
        List<SymbolButton> copies = new List<SymbolButton>();
        Vector3[] positions = new Vector3[rows + 1];
        List<Symbol> symbolsToShow = new List<Symbol>();

        for (int row = 0; row < rows; row++)
        {
            var original = slotGrid.GetSymbolInstance(col, row);
            originals[row] = original;
            positions[row + 1] = original.transform.localPosition;

            GameObject copyObj = GameObject.Instantiate(original.gameObject, original.transform.parent);
            copyObj.transform.localPosition = positions[row + 1];
            copies.Add(copyObj.GetComponent<SymbolButton>());

            original.gameObject.SetActive(false);
            copyObj.SetActive(true);
        }

        for (int row = symbolSteps - 1; row >= 0; row--)
            symbolsToShow.Add(slotGrid.GetSymbolFromFullGrid(col, row));

        float spacing = positions[1].y - positions[2].y;

        GameObject additional = GameObject.Instantiate(
            slotGrid.GetSymbolInstance(col, 0).gameObject,
            slotGrid.GetSymbolInstance(col, 0).transform.parent);
        additional.SetActive(true);
        copies.Add(additional.GetComponent<SymbolButton>());
        positions[0] = positions[1] + new Vector3(0, spacing, 0);

        try
        {
            for (int step = 0; step < symbolSteps; step++)
            {
                float elapsed = 0f;
                SymbolButton symbolButtonToMove = copies[^1];
                copies.RemoveAt(copies.Count - 1);
                symbolButtonToMove.SetSymbol(symbolsToShow[step]);
                symbolButtonToMove.transform.localPosition = positions[0];
                copies.Insert(0, symbolButtonToMove);

                while (elapsed < symbolStepDuration * gameData.animationSpeed)
                {
                    float t = Mathf.Clamp01(elapsed / symbolStepDuration / gameData.animationSpeed);
                    float offset = Mathf.Lerp(0, spacing, t);

                    for (int i = 0; i < rows + 1; i++)
                        copies[i].transform.localPosition = positions[i] - new Vector3(0f, offset, 0f);

                    await Awaitable.NextFrameAsync(ct);
                    elapsed += Time.deltaTime;
                }
            }
        }
        finally
        {
            for (int row = 0; row < rows; row++)
            {
                originals[row].SetSymbol(slotGrid.GetSymbol(col, row));
                originals[row].gameObject.SetActive(true);
            }

            foreach (var copy in copies)
                GameObject.Destroy(copy.gameObject);
            copies.Clear();
        }
    }
}
