using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotGridTestCase", menuName = "GridTestCase")]
public class SlotGridTestCase : ScriptableObject
{
    public int columns; // Number of columns (needed for proper display)
    public List<GridRow> grid; // Each row is a List<int>
    public int expectedResult;
}

[Serializable]
public class GridRow
{
    public List<int> rowValues; // Represents a row
}
