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
    
    
    public GameObject slotSymbolPrefab;     
    public GameObject slotPanel;

    public GameObject linePrefab;
    
    private WinningLineDrawer winningLineDrawer;
    private WinningPositionsDrawer winningPositionsDrawer;
    private CoroutineTracker coroutineTracker;
    
    private SlotGridManager slotGridCreator;
    void Start()
    {
        slotUIController = new SlotUIController();
        slotUIController.SetUp(gameData, slotMachine, this);

        SetFinishButton(false);
        UpdateButtons(SlotMode.ReadyForSpin);
        UpdateUI();

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

    public void SetUp(GameData gameData, SlotMachine slotMachine, SlotGrid slotGrid)
    {
        this.gameData = gameData;
        this.slotMachine = slotMachine;
        coroutineTracker = new CoroutineTracker(this, EnableButtons);
        winningLineDrawer = new WinningLineDrawer();
        winningLineDrawer.SetUp(this.transform, linePrefab, coroutineTracker);

        winningPositionsDrawer = new WinningPositionsDrawer();
        winningPositionsDrawer.SetUp(this.transform, coroutineTracker);
        
        slotGridCreator = new SlotGridManager(slotGrid, slotPanel, slotSymbolPrefab, slotUIController);
    }

    public void DrawWinningLines(List<(List<(int, int)> line, Symbol symbol)> winningLines, SlotGrid slotGrid)
    {
        DisableButtons();
        winningLineDrawer.DrawWinningLines(winningLines, slotGrid);
    }

    public void AnimatePositions(Dictionary<Symbol, SymbolPositions> playingPositions, SlotGrid slotGrid)
    {
        DisableButtons();
        winningPositionsDrawer.AnimatePositions(playingPositions, slotGrid);
    }

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
    public void DisableButtons()
    {
        UpdateButtons(SlotMode.AllDisabled);
    }

    public void EnableButtons()
    {
        UpdateButtons(SlotMode.WaitingForConfirm);
    }
    public void UpdateButtons(SlotMode mode)
    {
        spinButton.interactable = (mode == SlotMode.ReadyForSpin);
        confirmButton.interactable = (mode == SlotMode.WaitingForConfirm);
        changeButton.interactable = (mode == SlotMode.WaitingForConfirm);
        increaseButton.interactable = (mode == SlotMode.ReadyForSpin);
        decreaseButton.interactable = (mode == SlotMode.ReadyForSpin);
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

    public void SetUpGrid()
    {
        slotGridCreator.SetUpSlotUI();
    }
    
    public void UpdateSingleSymbol(int row, int col, Symbol newSymbol)
    {
        slotGridCreator.UpdateSingleSymbol(row, col, newSymbol);
    }
}
