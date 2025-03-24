using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningLineDrawer
{
    private GameObject linePrefab;  // Prefab with LineRenderer
    private List<GameObject> activeLines = new List<GameObject>();
    private MonoBehaviour coroutineRunner;
    
    private Transform parent;
    public void Setup(Transform parent, MonoBehaviour coroutineRunner, GameObject linePrefab)
    {
        this.parent = parent;
        this.coroutineRunner = coroutineRunner;
        this.linePrefab = linePrefab;
    }

    public void DrawWinningLines(List<List<(int,int)>> winningLines, SlotGrid slotGrid)
    {
        ClearLines();  // Remove previous lines

        foreach (var line in winningLines)
        {
            if (line.Count < 2) continue; // Skip invalid lines

            GameObject newLineObj = Object.Instantiate(linePrefab, parent);
            LineRenderer lr = newLineObj.GetComponent<LineRenderer>();

            // Set line color
            Color randomColor = new Color(Random.value, Random.value, Random.value);
            lr.startColor = lr.endColor = randomColor;

            coroutineRunner.StartCoroutine(DrawLineSegments(lr, line, slotGrid));

            activeLines.Add(newLineObj);
        }
    }

    private IEnumerator DrawLineSegments(LineRenderer lr, List<(int row, int column)> line, SlotGrid slotGrid)
    {
        lr.positionCount = 0;

        for (int i = 0; i < line.Count; i++)
        {
            GameObject symbolInstance = slotGrid.GetSymbolInstance(line[i].row, line[i].column);

            if (symbolInstance == null) continue;

            Vector3 pos = symbolInstance.transform.position;
            lr.positionCount = i + 1;
            lr.SetPosition(i, pos);

            yield return new WaitForSeconds(0.5f);
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