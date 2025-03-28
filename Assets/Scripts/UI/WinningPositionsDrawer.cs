using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningPositionsDrawer
{
    private MonoBehaviour coroutineRunner;
    private Transform parent;
    private float animationDuration = 0.3f;
    private float scaleMultiplier = 1.5f;

    public WinningPositionsDrawer() { }

    public void Setup(Transform parent, MonoBehaviour coroutineRunner)
    {
        this.parent = parent;
        this.coroutineRunner = coroutineRunner;
    }

    public void AnimatePositions(Dictionary<Symbol, List<(int, int)>> playingPositions, SlotGrid slotGrid)
    {
        coroutineRunner.StartCoroutine(AnimateWinningSymbolsSequentially(playingPositions, slotGrid));
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

        // Scale up
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

        // Scale down
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
