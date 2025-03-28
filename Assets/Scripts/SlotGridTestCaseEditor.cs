using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SlotGridTestCase))]
public class SlotGridTestCaseEditor : Editor
{
    private SlotGridTestCase testCase;
    private SerializedProperty gridProperty;
    private SerializedProperty columnsProperty;

    private void OnEnable()
    {
        testCase = (SlotGridTestCase)target;
        gridProperty = serializedObject.FindProperty("grid");
        columnsProperty = serializedObject.FindProperty("columns");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Input field for columns
        EditorGUILayout.PropertyField(columnsProperty);

        // Ensure row count matches the number of elements
        int rowCount = gridProperty.arraySize;
        int newRowCount = EditorGUILayout.IntField("Rows", rowCount);

        if (newRowCount != rowCount)
        {
            // Add or remove rows as needed
            while (gridProperty.arraySize < newRowCount)
            {
                gridProperty.InsertArrayElementAtIndex(gridProperty.arraySize);
                SerializedProperty newRow = gridProperty.GetArrayElementAtIndex(gridProperty.arraySize - 1);
                newRow.FindPropertyRelative("rowValues").arraySize = testCase.columns;
            }
            while (gridProperty.arraySize > newRowCount)
            {
                gridProperty.arraySize--;
            }
        }

        // Display grid in a table format with rows and columns
        for (int r = 0; r < gridProperty.arraySize; r++)
        {
            SerializedProperty row = gridProperty.GetArrayElementAtIndex(r);
            SerializedProperty rowValues = row.FindPropertyRelative("rowValues");

            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < testCase.columns; c++)
            {
                if (rowValues.arraySize < testCase.columns)
                {
                    rowValues.arraySize = testCase.columns;
                }
                SerializedProperty value = rowValues.GetArrayElementAtIndex(c);
                EditorGUILayout.PropertyField(value, GUIContent.none, GUILayout.Width(30));
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