using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotGridManager
{
    private GameObject slotPanel;
    private GameObject slotSymbolPrefab;
    private SlotGrid slotGrid;
    private SlotUIController slotUIController;
    private GameObject columnPrefab;
    private List<GameObject> columnContainers = new List<GameObject>();

    public SlotGridManager(GameObject slotPanel, GameObject slotSymbolPrefab, SlotGrid slotGrid, SlotUIController slotUIController, GameObject columnPrefab)
    {
        this.columnPrefab = columnPrefab;
        this.slotPanel = slotPanel;
        this.slotSymbolPrefab = slotSymbolPrefab;
        this.slotGrid = slotGrid;
        this.slotUIController = slotUIController;
    }

    public void Setup(List<CharacterStat> columnBuffs)
    {
        SetUpSlotUI(columnBuffs);
    }

    /*public List<GameObject> GetColumnContainers()
    {
        return columnContainers;
    }*/

    public void SetUpSlotUI(List<CharacterStat> columnBuffs)
    {
        //ClearPreviousUI();

        

        CreateColumnContainers();
        DisableColumnButtons();
        PopulateColumns(columnBuffs);
        DisableSymbolButtons();
        PopulateSymbolButtons();
        
        foreach (GameObject columnContainer in columnContainers)
        {
            CustomVerticalLayoutGroup layoutGroup = columnContainer.GetComponent<CustomVerticalLayoutGroup>();
            if (layoutGroup != null)
            {
                layoutGroup.ApplyLayout();
            }
        }
        CustomHorizontalLayoutGroup hlg = slotPanel.GetComponent<CustomHorizontalLayoutGroup>();
        hlg.ApplyLayout();
    }

    
    public void DisableSymbolButtons()
    {
        SymbolButton[,] symbolButtons = slotGrid.GetSymbolButtons();
        foreach (SymbolButton symbolButton in symbolButtons)
        {
            if (symbolButton != null)
                symbolButton.DisableButton();
        }
    }

    public void EnableSymbolButtons()
    {
        SymbolButton[,] symbolButtons = slotGrid.GetSymbolButtons();
        foreach (SymbolButton symbolButton in symbolButtons)
        {
            if (symbolButton != null)
                symbolButton.EnableButton();
        }
    }

    public void DisableColumnButtons()
    {
        foreach (GameObject column in columnContainers)
        {
            column.GetComponent<Button>().interactable = false;
        }
    }

    public void EnableColumnButtons()
    {
        foreach (GameObject column in columnContainers)
        {
            column.GetComponent<Button>().interactable = true;
        }
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
        int columnCount = slotGrid.GetColumnRowLength().columns;

        for (int col = 0; col < columnCount; col++)
        {
            GameObject columnContainer = Object.Instantiate(columnPrefab, slotPanel.transform);
            columnContainer.name = "Column " + col;
            columnContainer.transform.SetParent(slotPanel.transform, false);
            var cl = col;
            columnContainer.GetComponent<Button>().onClick.AddListener(() => slotUIController.ChangeColumn(cl));
            
            columnContainers.Add(columnContainer);
        }
    }

    private void PopulateColumns(List<CharacterStat> columnBuffs)
    {
        int columnCount = slotGrid.GetColumnRowLength().columns;

        for (int col = 0; col < columnCount; col++)
        {
            int buff = columnBuffs.ElementAtOrDefault(col)!.ValueInt;
            int actualRows = slotGrid.GetColumnRowLength().rows;

            for (int row = 0; row < actualRows; row++)
            {
                if (slotGrid.IsValidPosition(col, row))
                {
                    CreateSymbol(col, row);
                }
            }
        }
    }

    private void CreateSymbol(int col, int row)
    {
        SymbolButton newSymbol = Object.Instantiate(slotSymbolPrefab, columnContainers[col].transform).GetComponent<SymbolButton>();
        newSymbol.SetUp(col, row);
        newSymbol.GetComponent<Button>().onClick.AddListener(() => slotUIController.ChangeSymbol(col, row));
        slotGrid.RegisterSymbolInstance(col, row, newSymbol);
    }

    public void PopulateSymbolButtons()
    {
        SymbolButton[,] symbolButtons = slotGrid.GetSymbolButtons();
        foreach (SymbolButton symbolButton in symbolButtons)
        {
            if (symbolButton !=  null)
                symbolButton.SetSymbol(slotGrid.GetSymbol(symbolButton.col, symbolButton.row));
        }
    }

    public void ChangeSingleSymbol(int col, int row, Symbol newSymbol)
    {
        if (!slotGrid.IsValidPosition(col, row)) return;

        slotGrid.SetSymbol(col, row, newSymbol);

        SymbolButton symbolInstance = slotGrid.GetSymbolInstance(col, row);
        if (symbolInstance != null)
        {
            symbolInstance.GetComponent<SymbolButton>().SetSymbol(newSymbol);
        }
    }
}
