using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningPositionsDrawer
{
    private MonoBehaviour coroutineRunner;
    private CoroutineTracker coroutineTracker;
    private Transform parent;
    private float animationDuration = 0.3f;
    private float scaleMultiplier = 1.5f;

    public WinningPositionsDrawer(MonoBehaviour runner, System.Action onAnimationsFinished)
    {
        this.coroutineRunner = runner;
        this.coroutineTracker = new CoroutineTracker(runner, onAnimationsFinished);
    }

    public void Setup(Transform parent)
    {
        this.parent = parent;
    }

    public void AnimatePositions(Dictionary<Symbol, List<(int, int)>> playingPositions, SlotGrid slotGrid)
    {
        coroutineTracker.StartTrackedCoroutine(AnimateWinningSymbolsSequentially(playingPositions, slotGrid));
    }

    private IEnumerator AnimateWinningSymbolsSequentially(Dictionary<Symbol, List<(int, int)>> playingPositions, SlotGrid slotGrid)
    {
        foreach (var entry in playingPositions)
        {
            List<GameObject> symbolInstances = new List<GameObject>();

            foreach (var (row, column) in entry.Value)
            {
                GameObject symbolInstance = slotGrid.GetSymbolInstance(row, column);
                if (symbolInstance != null)
                {
                    symbolInstances.Add(symbolInstance);
                }
            }

            if (symbolInstances.Count > 0)
            {
                yield return coroutineRunner.StartCoroutine(AnimateSymbolScaling(symbolInstances));
            }
        }
    }

    private IEnumerator AnimateSymbolScaling(List<GameObject> symbolInstances)
    {
        float elapsedTime = 0f;
        Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

        foreach (var symbol in symbolInstances)
        {
            originalScales[symbol] = symbol.transform.localScale;
        }

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float scaleFactor = Mathf.SmoothStep(1f, scaleMultiplier, elapsedTime / animationDuration);
            foreach (var symbol in symbolInstances)
            {
                symbol.transform.localScale = originalScales[symbol] * scaleFactor;
            }
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float scaleFactor = Mathf.SmoothStep(scaleMultiplier, 1f, elapsedTime / animationDuration);
            foreach (var symbol in symbolInstances)
            {
                symbol.transform.localScale = originalScales[symbol] * scaleFactor;
            }
            yield return null;
        }
    }
}
