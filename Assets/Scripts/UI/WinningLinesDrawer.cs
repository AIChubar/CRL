using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WinningLineDrawer
{
    private GameObject linePrefab;
    private List<GameObject> activeLines = new List<GameObject>();
    private Transform parent;
    private GameData gameData;

    private float lineFragmentDuration = 0.01f;

    public void SetUp(Transform parent, GameObject linePrefab, GameData gameData)
    {
        this.gameData = gameData;
        this.parent = parent;
        this.linePrefab = linePrefab;
    }

    public async Awaitable DrawWinningLinesAsync(List<(List<(int, int)> line, Symbol symbol)> winningLines, SlotGrid slotGrid, CancellationToken ct)
    {
        ClearLines();
        await DrawLinesSequentially(winningLines, slotGrid, ct);
    }

    private async Awaitable DrawLinesSequentially(List<(List<(int, int)> line, Symbol symbol)> winningLines, SlotGrid slotGrid, CancellationToken ct)
    {
        for (int i = 0; i < winningLines.Count; i++)
        {
            var line = winningLines[i].line;
            if (line.Count < 2) continue;

            GameObject newLineObj = Object.Instantiate(linePrefab, parent);
            LineRenderer lr = newLineObj.GetComponent<LineRenderer>();
            lr.startColor = lr.endColor = Color.gray;
            activeLines.Add(newLineObj);

            await DrawLineSegments(lr, line, slotGrid, ct);
        }
    }

    private async Awaitable DrawLineSegments(LineRenderer lr, List<(int row, int column)> line, SlotGrid slotGrid, CancellationToken ct)
    {
        lr.positionCount = 0;
        await Awaitable.WaitForSecondsAsync(lineFragmentDuration * gameData.animationSpeed, ct);

        for (int i = 0; i < line.Count; i++)
        {
            SymbolButton symbolInstance = slotGrid.GetSymbolInstance(line[i].row, line[i].column);
            if (symbolInstance == null) continue;

            lr.positionCount = i + 1;
            lr.SetPosition(i, symbolInstance.transform.position);

            await Awaitable.WaitForSecondsAsync(lineFragmentDuration * gameData.animationSpeed, ct);
        }
    }

    public void ClearLines()
    {
        foreach (var line in activeLines)
            Object.Destroy(line);
        activeLines.Clear();
    }
}
