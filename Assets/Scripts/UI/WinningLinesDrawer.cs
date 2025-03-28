using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningLineDrawer
{
    private GameObject linePrefab;  // Prefab with LineRenderer
    private List<GameObject> activeLines = new List<GameObject>();
    private MonoBehaviour coroutineRunner;
    private Transform parent;
    private List<Color> predefinedColors;

    public WinningLineDrawer()
    {
        // Define a list of 30 distinct colors
        predefinedColors = new List<Color>
        {
            Color.red, Color.blue, Color.green, Color.yellow, Color.cyan,
            Color.magenta, Color.white, new Color(1.0f, 0.5f, 0.0f), // Orange
            new Color(0.5f, 0.0f, 0.5f), // Purple
            new Color(0.0f, 0.5f, 0.5f), // Teal
            Color.gray, Color.black, new Color(0.5f, 0.25f, 0.0f), // Brown
            new Color(0.75f, 0.75f, 0.0f), // Olive
            new Color(0.5f, 0.5f, 1.0f), // Light blue
            new Color(1.0f, 0.5f, 0.5f), // Light red
            new Color(0.5f, 1.0f, 0.5f), // Light green
            new Color(1.0f, 0.75f, 0.0f), // Gold
            new Color(0.5f, 0.5f, 0.0f), // Dark olive
            new Color(0.75f, 0.0f, 0.75f), // Deep purple
            new Color(0.25f, 0.25f, 0.25f), // Dark gray
            new Color(1.0f, 0.25f, 0.25f), // Bright red
            new Color(0.0f, 0.25f, 1.0f), // Vivid blue
            new Color(0.25f, 1.0f, 0.25f), // Vivid green
            new Color(0.75f, 0.5f, 0.25f), // Warm brown
            new Color(0.5f, 0.75f, 1.0f), // Sky blue
            new Color(1.0f, 1.0f, 0.5f), // Soft yellow
            new Color(0.5f, 1.0f, 1.0f), // Aqua
            new Color(1.0f, 0.5f, 1.0f)  // Pink
        };
    }

    public void Setup(Transform parent, MonoBehaviour coroutineRunner, GameObject linePrefab)
    {
        this.parent = parent;
        this.coroutineRunner = coroutineRunner;
        this.linePrefab = linePrefab;
    }

    public void DrawWinningLines(List<(List<(int, int)> line, Symbol symbol)> winningLines, SlotGrid slotGrid)
    {
        ClearLines();  // Remove previous lines
        coroutineRunner.StartCoroutine(DrawLinesSequentially(winningLines, slotGrid));
    }

    private IEnumerator DrawLinesSequentially(List<(List<(int, int)> line, Symbol symbol)> winningLines, SlotGrid slotGrid)
    {
        for (int i = 0; i < winningLines.Count; i++)
        {
            var line = winningLines[i].line;
            if (line.Count < 2) continue; // Skip invalid lines

            GameObject newLineObj = Object.Instantiate(linePrefab, parent);
            LineRenderer lr = newLineObj.GetComponent<LineRenderer>();

            // Set line color based on predefined list
            Color lineColor = predefinedColors[i % predefinedColors.Count];
            lr.startColor = lr.endColor = lineColor;

            activeLines.Add(newLineObj);

            yield return coroutineRunner.StartCoroutine(DrawLineSegments(lr, line, slotGrid));
        }
    }

    private IEnumerator DrawLineSegments(LineRenderer lr, List<(int row, int column)> line, SlotGrid slotGrid)
    {
        lr.positionCount = 0;
        yield return new WaitForSeconds(0.1f); // temporary fix (To do) (symbols are created at a certain pos initially)
        for (int i = 0; i < line.Count; i++)
        {
            GameObject symbolInstance = slotGrid.GetSymbolInstance(line[i].row, line[i].column);

            if (symbolInstance == null)
            {
                Debug.LogError($"Symbol at ({line[i].row}, {line[i].column}) is null!");
                continue;
            }

            Vector3 pos = symbolInstance.transform.position;
            lr.positionCount = i + 1;
            lr.SetPosition(i, pos);

            yield return new WaitForSeconds(0.1f);
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
