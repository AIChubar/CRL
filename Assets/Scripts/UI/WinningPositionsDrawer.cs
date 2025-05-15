using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningPositionsDrawer
{
    private CoroutineTracker coroutineTracker;
    private Transform parent;
    private float pulseDuration = 0.4f;
    private float scaleMultiplier = 1.5f;
    private GameData gameData;

    public void SetUp(Transform parent, CoroutineTracker coroutineTracker, GameData gameData)
    {
        this.gameData = gameData;
        this.coroutineTracker = coroutineTracker;
        this.parent = parent;
    }

    public void AnimatePositions(Dictionary<Symbol, SymbolPositions> playingPositions, SlotGrid slotGrid)
    {
        coroutineTracker.StartTrackedCoroutine(AnimateWinningSymbolsSequentially(playingPositions, slotGrid));
    }

    private IEnumerator AnimateWinningSymbolsSequentially(Dictionary<Symbol, SymbolPositions> playingPositions, SlotGrid slotGrid)
    {
        foreach (var entry in playingPositions)
        {
            if(entry.Value.WasShown)
                continue;
            entry.Value.WasShown = true;
            List<SymbolButton> symbolInstances = new List<SymbolButton>();

            foreach (var (row, column) in entry.Value.Positions)
            {
                SymbolButton symbolInstance = slotGrid.GetSymbolInstance(row, column);
                if (symbolInstance != null)
                {
                    symbolInstances.Add(symbolInstance);
                }
            }

            if (symbolInstances.Count > 0)
            {
                yield return coroutineTracker.StartTrackedCoroutine(AnimateSymbolScaling(symbolInstances));
            }
        }
    }

    private IEnumerator AnimateSymbolScaling(List<SymbolButton> symbolInstances)
    {
        float elapsedTime = 0f;
        Dictionary<SymbolButton, Vector3> originalScales = new Dictionary<SymbolButton, Vector3>();

        foreach (var symbol in symbolInstances)
        {
            originalScales[symbol] = symbol.transform.localScale;
        }

        while (elapsedTime < pulseDuration * gameData.animationSpeed)
        {
            elapsedTime += Time.deltaTime;
            float scaleFactor = Mathf.SmoothStep(1f, scaleMultiplier, elapsedTime / pulseDuration / gameData.animationSpeed);
            foreach (var symbol in symbolInstances)
            {
                symbol.transform.localScale = originalScales[symbol] * scaleFactor;
            }
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < pulseDuration  * gameData.animationSpeed)
        {
            elapsedTime += Time.deltaTime;
            float scaleFactor = Mathf.SmoothStep(scaleMultiplier, 1f, elapsedTime / pulseDuration / gameData.animationSpeed);
            foreach (var symbol in symbolInstances)
            {
                symbol.transform.localScale = originalScales[symbol] * scaleFactor;
            }
            yield return null;
        }
    }
}
