using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningLineDrawer
{
    private GameObject linePrefab;
    private List<GameObject> activeLines = new List<GameObject>();
    private CoroutineTracker coroutineTracker;
    private Transform parent;

    public WinningLineDrawer(MonoBehaviour runner, System.Action onAnimationsFinished)
    {
        this.coroutineTracker = new CoroutineTracker(runner, onAnimationsFinished);
    }

    public void Setup(Transform parent, GameObject linePrefab)
    {
        this.parent = parent;
        this.linePrefab = linePrefab;
    }

    public void DrawWinningLines(List<(List<(int, int)> line, Symbol symbol)> winningLines, SlotGrid slotGrid)
    {
        ClearLines();
        coroutineTracker.StartTrackedCoroutine(DrawLinesSequentially(winningLines, slotGrid));
    }

    private IEnumerator DrawLinesSequentially(List<(List<(int, int)> line, Symbol symbol)> winningLines, SlotGrid slotGrid)
    {
        for (int i = 0; i < winningLines.Count; i++)
        {
            var line = winningLines[i].line;
            if (line.Count < 2) continue;

            GameObject newLineObj = Object.Instantiate(linePrefab, parent);
            LineRenderer lr = newLineObj.GetComponent<LineRenderer>();

            lr.startColor = lr.endColor = Color.gray;
            activeLines.Add(newLineObj);

            yield return coroutineTracker.StartTrackedCoroutine(DrawLineSegments(lr, line, slotGrid));
        }
    }

    private IEnumerator DrawLineSegments(LineRenderer lr, List<(int row, int column)> line, SlotGrid slotGrid)
    {
        lr.positionCount = 0;
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < line.Count; i++)
        {
            GameObject symbolInstance = slotGrid.GetSymbolInstance(line[i].row, line[i].column);
            if (symbolInstance == null) continue;

            Vector3 pos = symbolInstance.transform.position;
            lr.positionCount = i + 1;
            lr.SetPosition(i, pos);

            yield return new WaitForSeconds(0.4f);
        }
    }

    public void ClearLines()
    {
        foreach (var line in activeLines)
        {
            Object.Destroy(line);
        }
        activeLines.Clear();
    }
}
