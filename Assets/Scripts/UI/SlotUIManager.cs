using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public enum SlotMode
{
    ReadyForSpin = 100,
    ChangingSymbols = 200,
    WaitingForConfirm = 300,
    AllDisabled = 400,
}


public class SlotUIManager : MonoBehaviour
{
    public List<int> betAmounts = new List<int>() { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 140, 160, 180, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000 };
    public float winCoef;
    public bool isChangingSymbol = false;

    public TextMeshProUGUI moneyText, targetText, wagerText, spinsText, betText, resultText, changePriceText, instructionText, goldText;
    public Button changeButton, confirmButton, spinButton, increaseButton, decreaseButton, restartButton, pauseRestartButton, /*nextLevelButton,*/ continueButton, toMenuButton, pauseToMenuButton, toShopButton;
    [SerializeField] private Button finishRoundButton;
    [SerializeField] private TextMeshProUGUI finishRoundText;
    public PauseManager pauseManager;

    private SlotUIController slotUIController;
    private GameData gameData;
    private SlotMachine slotMachine;
    private float currentWin;
    
    private SlotGrid slotGrid;
    
    public GameObject slotSymbolPrefab;     
    public GameObject slotPanel;

    public GameObject linePrefab;
    
    private WinningLineDrawer winningLineDrawer;
    void Start()
    {

        slotUIController = new SlotUIController();
        slotUIController.Setup(gameData, slotMachine, this);

        SetFinishButton(false);
        UpdateButtons(SlotMode.ReadyForSpin);
        UpdateUI();

        // Attach button listeners dynamically
        spinButton.onClick.AddListener(slotUIController.Spin);
        confirmButton.onClick.AddListener(slotUIController.ConfirmSpin);
        changeButton.onClick.AddListener(slotUIController.StartChangingSymbol);
        increaseButton.onClick.AddListener(slotUIController.IncreaseBet);
        decreaseButton.onClick.AddListener(slotUIController.DecreaseBet);
        restartButton.onClick.AddListener(slotUIController.Restart);
        pauseRestartButton.onClick.AddListener(slotUIController.Restart);
        continueButton.onClick.AddListener(slotUIController.Continue);
        toMenuButton.onClick.AddListener(slotUIController.ToMenu);
        pauseToMenuButton.onClick.AddListener(slotUIController.ToMenu);
        toShopButton.onClick.AddListener(slotUIController.ToShop);
        finishRoundButton.onClick.AddListener(slotUIController.FinishRound);
    }

    public void Setup(GameData gameData, SlotMachine slotMachine, SlotGrid slotGrid)
    {
        this.gameData = gameData;
        this.slotMachine = slotMachine;
        this.slotGrid = slotGrid;
        winningLineDrawer = new WinningLineDrawer();
        winningLineDrawer.Setup(this.transform,this, linePrefab);
    }
    
    public void DrawWinningLines(List<List<(int,int)>> winningLines, SlotGrid slotGrid) => winningLineDrawer.DrawWinningLines(winningLines, slotGrid);


    public void UpdateUI()
    {
        moneyText.text = $"Money: ${gameData.money:0.00}";
        spinsText.text = $"Spins Left: {gameData.spinsLeft}";
        betText.text = $"Bet: ${gameData.betAmount}";
        targetText.text = $"Target Money: ${gameData.targetMoney:0.00}";
        wagerText.text = $"Wager Left: ${gameData.wagerLeft}";
        changePriceText.text = $"${gameData.changePrice}";
        goldText.text = $"Gold: ${gameData.gold}";
    }

    public void UpdateButtons(SlotMode mode)
    {
        spinButton.interactable = (mode == SlotMode.ReadyForSpin);
        confirmButton.interactable = (mode == SlotMode.WaitingForConfirm);
        changeButton.interactable = (mode == SlotMode.WaitingForConfirm);
        increaseButton.interactable = (mode == SlotMode.ReadyForSpin);
        decreaseButton.interactable = (mode == SlotMode.ReadyForSpin);
    }
    public void CreateSymbolUI(GameObject slotSymbolPrefab, GameObject slotPanel, int row, int col, float cellWidth, float cellHeight) //not efficient
    {
        GameObject newSymbol = Instantiate(slotSymbolPrefab, slotPanel.transform);
        newSymbol.GetComponent<TMP_Text>().text = slotGrid.GetSymbol(row, col).ch;

        RectTransform rectTransform = newSymbol.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);
        rectTransform.localScale = Vector3.one;

        int r = row, c = col;
        newSymbol.GetComponent<Button>().onClick.AddListener(() => slotUIController.ChangeSymbol(r, c));
        slotGrid.RegisterSymbolInstance(row,col,newSymbol);
    }
    
    
    public void SetFinishButton(bool interactable)
    {
        finishRoundButton.interactable = interactable;
        finishRoundText.color = new Color(0, 0, 0, interactable ? 1f : 0.4f);
    }
    
    public void UpdateResultText(string result)
    {
        resultText.text = result;
        UpdateUI();

    }
    
    public void UpdateInstructionText(string result)
    {
        instructionText.text = result;
        UpdateUI();
    }
    
    
    public void UpdateSlotUI() // not efficient
    {
        foreach (Transform child in slotPanel.transform)
        {
            Destroy(child.gameObject);
        }

        GridLayoutGroup grid = slotPanel.GetComponent<GridLayoutGroup>();
        SetupGridLayout(slotGrid.GetRowColumnLength().rows, slotGrid.GetRowColumnLength().columns, grid);



        for (int row = 0; row < slotGrid.GetRowColumnLength().rows; row++)
        {
            for (int col = 0; col < slotGrid.GetRowColumnLength().columns; col++)
            {
                CreateSymbolUI(slotSymbolPrefab, slotPanel, row, col, grid.cellSize.x, grid.cellSize.y);
            }
        }
    }
    
    private void SetupGridLayout(int rows, int columns, GridLayoutGroup grid)
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
    public void UpdateSingleSymbol(int row, int col, Symbol newSymbol)
    {
        slotGrid.SetSymbol(row, col, newSymbol);
        Transform symbolTransform = slotPanel.transform.GetChild(row * slotGrid.GetRowColumnLength().rows + col);
        symbolTransform.GetComponent<TMP_Text>().text = newSymbol.ch;
    }
}
