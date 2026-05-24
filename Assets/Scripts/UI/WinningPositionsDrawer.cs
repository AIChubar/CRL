using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WinningPositionsDrawer
{
    private Transform parent;
    private float pulseDuration = 0.4f;
    private float scaleMultiplier = 1.5f;
    private GameData gameData;

    public void SetUp(Transform parent, GameData gameData)
    {
        this.gameData = gameData;
        this.parent = parent;
    }

    public async Awaitable AnimatePositionsAsync(Dictionary<Symbol, SymbolPositions> playingPositions, SlotGrid slotGrid, CancellationToken ct)
    {
        foreach (var entry in playingPositions)
        {
            if (entry.Value.WasShown) continue;
            entry.Value.WasShown = true;

            var symbolInstances = new List<SymbolButton>();
            foreach (var (row, column) in entry.Value.Positions)
            {
                SymbolButton symbolInstance = slotGrid.GetSymbolInstance(row, column);
                if (symbolInstance != null)
                    symbolInstances.Add(symbolInstance);
            }

            if (symbolInstances.Count > 0)
                await AnimateSymbolScaling(symbolInstances, ct);
        }
    }

    private async Awaitable AnimateSymbolScaling(List<SymbolButton> symbolInstances, CancellationToken ct)
    {
        float elapsedTime = 0f;
        var originalScales = new Dictionary<SymbolButton, Vector3>();
        int animationDrawDiff = 1;

        foreach (var symbol in symbolInstances)
        {
            originalScales[symbol] = symbol.transform.localScale;
            Canvas symbolCanvas = symbol.GetComponent<Canvas>();
            if (symbolCanvas == null)
            {
                symbolCanvas = symbol.gameObject.AddComponent<Canvas>();
                symbolCanvas.overrideSorting = true;
            }
            symbolCanvas.sortingOrder = +animationDrawDiff;
        }

        while (elapsedTime < pulseDuration * gameData.animationSpeed)
        {
            elapsedTime += Time.deltaTime;
            float scaleFactor = Mathf.SmoothStep(1f, scaleMultiplier, elapsedTime / pulseDuration / gameData.animationSpeed);
            foreach (var symbol in symbolInstances)
                symbol.transform.localScale = originalScales[symbol] * scaleFactor;
            await Awaitable.NextFrameAsync(ct);
        }

        elapsedTime = 0f;

        while (elapsedTime < pulseDuration * gameData.animationSpeed)
        {
            elapsedTime += Time.deltaTime;
            float scaleFactor = Mathf.SmoothStep(scaleMultiplier, 1f, elapsedTime / pulseDuration / gameData.animationSpeed);
            foreach (var symbol in symbolInstances)
                symbol.transform.localScale = originalScales[symbol] * scaleFactor;
            await Awaitable.NextFrameAsync(ct);
        }

        foreach (var symbol in symbolInstances)
        {
            Canvas symbolCanvas = symbol.GetComponent<Canvas>();
            symbolCanvas.sortingOrder = -animationDrawDiff;
            symbolCanvas.overrideSorting = false;
        }
    }
}
