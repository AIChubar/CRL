using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatModifier))]
public class StatModifierEditor : Editor
{
    SerializedProperty isColumnEnabledProp;
    SerializedProperty columnIndexProp;
    SerializedProperty statTypeProp;
    SerializedProperty statModTypeProp;

    void OnEnable()
    {
        // Get references to the SerializedProperties
        isColumnEnabledProp = serializedObject.FindProperty("IsColumnSpecific");
        columnIndexProp = serializedObject.FindProperty("ColumnIndex");
        statTypeProp = serializedObject.FindProperty("StatType");
        statModTypeProp = serializedObject.FindProperty("StatModType");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default inspector fields excluding ColumnIndex and IsColumnSpecific
        DrawPropertiesExcluding(serializedObject, "ColumnIndex", "IsColumnSpecific");

        // Conditionally handle the StatModType and ColumnIndex fields based on StatType
        if (statTypeProp.enumValueIndex == (int)StatType.ColumnBuff)
        {
            // Automatically set StatModType to Flat if ColumnBuff is selected
            statModTypeProp.enumValueIndex = (int)StatModType.Flat;  // Ensure this assignment uses the correct index (Flat is 100)

            // Disable the StatModType field to prevent changes
            GUI.enabled = false;
        }


        // Re-enable GUI interaction for other fields
        GUI.enabled = true;

        // Conditionally draw the ColumnIndex field based on StatType being ColumnBuff
        if (statTypeProp.enumValueIndex == (int)StatType.ColumnBuff)
        {
            isColumnEnabledProp.boolValue = true;
            // Only draw the ColumnIndex field when StatType is ColumnBuff
            EditorGUILayout.PropertyField(columnIndexProp);
            columnIndexProp.intValue = Mathf.Clamp(columnIndexProp.intValue, 0, 2); // Ensure the value is between 0 and 2
        }

        serializedObject.ApplyModifiedProperties();
    }
}
