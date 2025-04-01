using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotGridManager
{
    private SlotGrid slotGrid;
    private GameObject slotPanel;
    private GameObject slotSymbolPrefab;
    private SlotUIController slotUIController;
    public SlotGridManager(SlotGrid slotGrid, GameObject slotPanel, GameObject slotSymbolPrefab, SlotUIController slotUIController)
    {
        this.slotGrid = slotGrid;
        this.slotPanel = slotPanel;
        this.slotSymbolPrefab = slotSymbolPrefab;
        this.slotUIController = slotUIController;
    }
    
    public void SetUpSlotUI() // not efficient
    {
        foreach (Transform child in slotPanel.transform)
        {
            Object.Destroy(child.gameObject);
        }

        GridLayoutGroup grid = slotPanel.GetComponent<GridLayoutGroup>();
        SetUpGridLayout(slotGrid.GetRowColumnLength().rows, slotGrid.GetRowColumnLength().columns, grid);



        for (int row = 0; row < slotGrid.GetRowColumnLength().rows; row++)
        {
            for (int col = 0; col < slotGrid.GetRowColumnLength().columns; col++)
            {
                CreateSymbolUI(slotSymbolPrefab, slotPanel, row, col, grid.cellSize.x, grid.cellSize.y);
            }
        }
    }
    
    private void SetUpGridLayout(int rows, int columns, GridLayoutGroup grid)
    {
        if (grid == null)
        {
            grid = slotPanel.AddComponent<GridLayoutGroup>();
        }
        float panelWidth = 1200f;
        float panelHeight = 750f;
        float maxCellWidth = panelWidth / columns;
        float maxCellHeight = panelHeight / rows;

        grid.cellSize = new Vector2(maxCellWidth - maxCellWidth / 10f, maxCellHeight - maxCellHeight / 10f);
        grid.spacing = new Vector2(maxCellWidth / 10f, maxCellHeight / 10f);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.childAlignment = TextAnchor.MiddleCenter;
    }
    
    public void CreateSymbolUI(GameObject slotSymbolPrefab, GameObject slotPanel, int row, int col, float cellWidth, float cellHeight) //not efficient
    {
        GameObject newSymbol = Object.Instantiate(slotSymbolPrefab, slotPanel.transform);
        newSymbol.GetComponent<TMP_Text>().text = slotGrid.GetSymbol(row, col).ch;

        RectTransform rectTransform = newSymbol.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);
        rectTransform.localScale = Vector3.one;

        int r = row, c = col;
        newSymbol.GetComponent<Button>().onClick.AddListener(() => slotUIController.ChangeSymbol(r, c));
        slotGrid.RegisterSymbolInstance(row,col,newSymbol);
    }
    
    public void UpdateSingleSymbol(int row, int col, Symbol newSymbol)
    {
        slotGrid.SetSymbol(row, col, newSymbol);
        Transform symbolTransform = slotPanel.transform.GetChild(row * slotGrid.GetRowColumnLength().rows + col);
        symbolTransform.GetComponent<TMP_Text>().text = newSymbol.ch;
    }

}
