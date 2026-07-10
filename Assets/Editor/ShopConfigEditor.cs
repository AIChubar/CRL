using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ShopConfig))]
public class ShopConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShopConfig config = (ShopConfig)target;

        if (GUILayout.Button("Populate with All Items"))
        {
            string[] guids = AssetDatabase.FindAssets("t:Item");
            config.allItems = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<Item>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(item => item != null)
                .ToList();

            EditorUtility.SetDirty(config);
            Debug.Log($"Populated with {config.allItems.Count} items.");
        }
    }
}