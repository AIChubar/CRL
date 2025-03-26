using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SlotGridTestCase))]
public class SlotGridTestCaseEditor : Editor
{
    private void OnEnable()
    {
        SlotGridTestCase testCase = (SlotGridTestCase)target;

        // Initialize grid if it's null or empty
        if (testCase.grid == null)
        {
            testCase.grid = new List<GridRow>();
        }

        if (testCase.grid.Count == 0)
        {
            for (int i = 0; i < 3; i++) // Example: 3 rows, adjust as needed
            {
                testCase.grid.Add(new GridRow { rowValues = new List<int>(new int[testCase.columns]) });
            }
        }
    }

    public override void OnInspectorGUI()
    {
        SlotGridTestCase testCase = (SlotGridTestCase)target;

        serializedObject.Update();

        // Input field for columns
        testCase.columns = EditorGUILayout.IntField("Columns", testCase.columns);

        // Ensure grid has at least one row
        if (testCase.grid == null)
        {
            testCase.grid = new List<GridRow>();
        }

        // Ensure row count matches the number of elements
        int rowCount = testCase.grid.Count;
        int newRowCount = EditorGUILayout.IntField("Rows", rowCount);

        if (newRowCount != rowCount)
        {
            // Add or remove rows as needed
            while (testCase.grid.Count < newRowCount)
            {
                testCase.grid.Add(new GridRow { rowValues = new List<int>(new int[testCase.columns]) });
            }
            while (testCase.grid.Count > newRowCount)
            {
                testCase.grid.RemoveAt(testCase.grid.Count - 1);
            }
        }

        // Display grid in a table format with rows and columns
        for (int r = 0; r < testCase.grid.Count; r++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < testCase.columns; c++)
            {
                if (testCase.grid[r].rowValues.Count < testCase.columns)
                {
                    testCase.grid[r].rowValues.Add(0); // Fill missing columns
                }
                testCase.grid[r].rowValues[c] = EditorGUILayout.IntField(testCase.grid[r].rowValues[c], GUILayout.Width(30));
            }
            EditorGUILayout.EndHorizontal();
        }

        // Display Expected Result field
        testCase.expectedResult = EditorGUILayout.IntField("Expected Result", testCase.expectedResult);

        serializedObject.ApplyModifiedProperties();

        // Force the inspector to repaint for immediate changes
        Repaint();
    }
}
