using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotGridManager
{
    private GameObject slotPanel;
    private GameObject slotSymbolPrefab;
    private SlotGrid slotGrid;
    private SlotUIController slotUIController;

    private List<GameObject> columnContainers = new List<GameObject>();

    public SlotGridManager(GameObject slotPanel, GameObject slotSymbolPrefab, SlotGrid slotGrid, SlotUIController slotUIController)
    {
        this.slotPanel = slotPanel;
        this.slotSymbolPrefab = slotSymbolPrefab;
        this.slotGrid = slotGrid;
        this.slotUIController = slotUIController;
    }

    public void SetUpSlotUI(List<CharacterStat> columnBuffs)
    {
        ClearPreviousUI();

        // Ensure the parent panel has a HorizontalLayoutGroup
        HorizontalLayoutGroup hlg = slotPanel.GetComponent<HorizontalLayoutGroup>();
        if (hlg == null)
        {
            hlg = slotPanel.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.spacing = 20; // Adjust spacing between columns
            hlg.childControlWidth = true;
            hlg.childForceExpandWidth = false;
        }

        CreateColumnContainers();
        PopulateColumns(columnBuffs);
    }


    private void ClearPreviousUI()
    {
        foreach (Transform child in slotPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        columnContainers.Clear();
    }

    private void CreateColumnContainers()
    {
        int columnCount = slotGrid.GetRowColumnLength().columns;

        for (int col = 0; col < columnCount; col++)
        {
            GameObject columnContainer = new GameObject("Column_" + col);
            columnContainer.transform.SetParent(slotPanel.transform, false);

            VerticalLayoutGroup vlg = columnContainer.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.spacing = 10;
            vlg.childControlHeight = true;
            vlg.childForceExpandHeight = false;

            columnContainers.Add(columnContainer);
        }
    }

    private void PopulateColumns(List<CharacterStat> columnBuffs)
    {
        int columnCount = slotGrid.GetRowColumnLength().columns;

        for (int col = 0; col < columnCount; col++)
        {
            int buff = (int)columnBuffs.ElementAtOrDefault(col)!.ValueInt; // Prevent out-of-range errors
            int actualRows = slotGrid.GetRowColumnLength().rows + buff;

            for (int row = 0; row < actualRows; row++)
            {
                if (slotGrid.IsValidPosition(row, col))
                {
                    CreateSymbol(row, col);
                }
            }
        }
    }

    private void CreateSymbol(int row, int col)
    {
        GameObject newSymbol = GameObject.Instantiate(slotSymbolPrefab, columnContainers[col].transform);
        newSymbol.GetComponent<TMP_Text>().text = slotGrid.GetSymbol(row, col).ch;
        newSymbol.GetComponent<Button>().onClick.AddListener(() => slotUIController.ChangeSymbol(row, col));

        slotGrid.RegisterSymbolInstance(row, col, newSymbol);
    }

    public void UpdateSingleSymbol(int row, int col, Symbol newSymbol)
    {
        if (!slotGrid.IsValidPosition(row, col)) return;

        slotGrid.SetSymbol(row, col, newSymbol);

        GameObject symbolInstance = slotGrid.GetSymbolInstance(row, col);
        if (symbolInstance != null)
        {
            symbolInstance.GetComponent<TMP_Text>().text = newSymbol.ch;
        }
    }
}
