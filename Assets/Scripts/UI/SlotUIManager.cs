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
    ChangingColumn = 250,
    WaitingForConfirm = 300,
    AllDisabled = 400,
}


public class SlotUIManager : MonoBehaviour
{
    public List<int> betAmounts = new List<int>() { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 140, 160, 180, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000 };
    public float winCoef;
    [HideInInspector] public SlotMode slotMode;

    public TextMeshProUGUI tokensText, moneyText, targetText, spinsText, betText, resultText, changePriceText, wildPriceText, instructionText, goldText;
    public Button changeButton, wildButton, confirmButton, spinButton, increaseButton, decreaseButton, restartButton, pauseRestartButton, /*nextLevelButton,*/ continueButton, toMenuButton, pauseToMenuButton, toShopButton;
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
    
    private SlotGridManager slotGridManager;
    
    public GameObject consumableButtonPrefab;
    public GameObject columnPrefab;
    private SlotSpinAnimator slotSpinAnimator;
    private SlotGrid slotGrid;
    public Transform consumableButtonContainer; // Assign UI Panel for items
    private List<ConsumableItemButton> consumableButtons = new List<ConsumableItemButton>();
    //private List<GameObject> columnContainers;

    public void SetUp(GameData gameData, SlotMachine slotMachine, SlotGrid slotGrid)
    {
        this.slotGrid = slotGrid;
        this.gameData = gameData;
        this.slotMachine = slotMachine;
        slotMode = SlotMode.ReadyForSpin;
        coroutineTracker = new CoroutineTracker(this, AnimationType.Result);
        winningLineDrawer = new WinningLineDrawer();
        winningLineDrawer.SetUp(this.transform, linePrefab, coroutineTracker, gameData);

        winningPositionsDrawer = new WinningPositionsDrawer();
        winningPositionsDrawer.SetUp(this.transform, coroutineTracker, gameData);

        slotSpinAnimator = new SlotSpinAnimator(slotGrid, this, gameData);
        
        slotUIController = new SlotUIController();
        slotUIController.SetUp(gameData, slotMachine, this);
        
        SetFinishButton(false);

        spinButton.onClick.AddListener(slotUIController.Spin);
        confirmButton.onClick.AddListener(slotUIController.ConfirmSpin);
        changeButton.onClick.AddListener(slotUIController.OnChangeButtonClick);
        wildButton.onClick.AddListener(slotUIController.OnWildButtonClick);
        increaseButton.onClick.AddListener(slotUIController.IncreaseBet);
        decreaseButton.onClick.AddListener(slotUIController.DecreaseBet);
        restartButton.onClick.AddListener(slotUIController.Restart);
        pauseRestartButton.onClick.AddListener(slotUIController.Restart);
        continueButton.onClick.AddListener(slotUIController.Continue);
        toMenuButton.onClick.AddListener(slotUIController.ToMenu);
        pauseToMenuButton.onClick.AddListener(slotUIController.ToMenu);
        toShopButton.onClick.AddListener(slotUIController.ToShop);
        finishRoundButton.onClick.AddListener(slotUIController.FinishRound);
        
        slotGridManager = new SlotGridManager(slotPanel, slotSymbolPrefab,slotGrid , slotUIController, columnPrefab);
        slotGridManager.Setup(gameData.columnBuffs);

        LoadConsumables();
        UpdateUI();

        UpdateButtons();

    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.instance.eventManager.OnConsumableItemUsed.RemoveListener(OnConsumableItemUsed);
        GameManager.instance.eventManager.OnCoroutineEnd.RemoveListener(OnCoroutineEnd);
    }

    private void SubscribeToEvents()
    {
        GameManager.instance.eventManager.OnConsumableItemUsed.AddListener(OnConsumableItemUsed);
        GameManager.instance.eventManager.OnCoroutineEnd.AddListener(OnCoroutineEnd);
    }

    private void OnCoroutineEnd(AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.Spin:
                DrawWinningLines(slotMachine.GetWinningLines());
                AnimatePositions(slotMachine.GetPlayingPositions());
                DisableButtons();
                break;
            case AnimationType.Result:
                slotMode = SlotMode.WaitingForConfirm;
                UpdateButtons();
                break;
        }
    }

    public void OnConsumableItemUsed(ConsumableItemType item)
    {
        switch (item)
        {
            case ConsumableItemType.ColumnRoll:
                StartChangingColumn();
                break;
            case ConsumableItemType.SymbolRoll:
                StartChangingSymbol();
                break;
            case ConsumableItemType.SymbolTypeRoll:
                break;
            default: break;
        }
    }
    public void StartChangingSymbol()
    {
        slotMode = SlotMode.ChangingSymbols;
        slotGridManager.EnableSymbolButtons();
        UpdateButtons();
        UpdateInstructionText("Pick symbol you want to change!");
    }
    
    public void StartChangingColumn()
    {
        slotMode = SlotMode.ChangingColumn;
        slotGridManager.EnableColumnButtons();
        UpdateButtons();
        UpdateInstructionText("Pick column you want to change!");
    }
    public void DrawWinningLines(List<(List<(int, int)> line, Symbol symbol)> winningLines)
    {
        winningLineDrawer.DrawWinningLines(winningLines, slotGrid);
    }

    public void AnimatePositions(Dictionary<Symbol, SymbolPositions> playingPositions)
    {
        winningPositionsDrawer.AnimatePositions(playingPositions, slotGrid);
    }
    
    private void LoadConsumables()
    {
        foreach (var item in gameData.consumableItems)
        {
            GameObject obj = Instantiate(consumableButtonPrefab, consumableButtonContainer);
            ConsumableItemButton button = obj.GetComponent<ConsumableItemButton>();
            button.Setup(item);
            consumableButtons.Add(button);
        }
    }

    public void UpdateUI()
    {
        moneyText.text = $"Balance: ${gameData.money:0.00}";
        tokensText.text = $"Tokens: {gameData.tokens}T";
        spinsText.text = $"Spins Left: {gameData.spinsLeft}";
        betText.text = $"Bet: ${gameData.betAmount}";
        targetText.text = $"Target Money: ${gameData.targetMoney:0.00}";
        changePriceText.text = $"{gameData.changePrice}T ";
        wildPriceText.text = $"{gameData.wildPrice}T ";
        goldText.text = $"Gold: {gameData.gold}";
    }
    public void DisableButtons()
    {
        slotMode = SlotMode.AllDisabled;
        UpdateButtons();
    }


    public void UpdateButtons() 
    {
        spinButton.interactable = (slotMode == SlotMode.ReadyForSpin);
        confirmButton.interactable = (slotMode == SlotMode.WaitingForConfirm);
        changeButton.interactable = (slotMode == SlotMode.WaitingForConfirm);
        wildButton.interactable = (slotMode == SlotMode.WaitingForConfirm);
        increaseButton.interactable = (slotMode == SlotMode.ReadyForSpin);
        decreaseButton.interactable = (slotMode == SlotMode.ReadyForSpin);
        consumableButtons.RemoveAll(button => button == null || button.gameObject == null);

        foreach (var consumableButton in consumableButtons)
        {
            if (slotMode == SlotMode.WaitingForConfirm)
                consumableButton.EnableButton();
            else
                consumableButton.DisableButton();
        }
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

    public void SetUpGridUI()
    {
        slotSpinAnimator.StartSpin();
        winningLineDrawer.ClearLines();
        slotGridManager.DisableSymbolButtons();
        slotGridManager.DisableColumnButtons();
        DisableButtons();
    }
    
    public void ChangeSingleSymbol(int row, int col, Symbol newSymbol)
    {
        slotGridManager.ChangeSingleSymbol(row, col, newSymbol);
        slotGridManager.DisableSymbolButtons();
        slotGridManager.DisableColumnButtons();
    }

}
