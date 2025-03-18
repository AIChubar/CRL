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
    public List<int> betAmounts =  new List<int>(){ 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 140, 160, 180, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000 };
    private float currentWin = 0;
    public bool isChangingSymbol = false; 
    public float winCoef;
    
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI wagerText;
    public TextMeshProUGUI spinsText;
    public TextMeshProUGUI betText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI changePriceText;

    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI goldText;

    public Button changeButton;
    public Button confirmButton;
    public Button spinButton;
    public Button increaseButton;
    public Button decreaseButton;
    
    [SerializeField] private Button finishRoundButton;
    [SerializeField] private TextMeshProUGUI finishRoundText;
    
    private SlotUIController slotUIController;
    public PauseManager pauseManager;

    public void UpdateUI()
    {
        moneyText.text = "Money: $" + GameManager.instance.gameData.money.ToString("0.00");
        spinsText.text = "Spins Left: " + GameManager.instance.gameData.spinsLeft.ToString();
        betText.text = "Bet: $" + GameManager.instance.gameData.betAmount.ToString();
        targetText.text = "Target Money: $" + GameManager.instance.gameData.targetMoney.ToString("0.00");
        wagerText.text = "Wager Left: $" + GameManager.instance.gameData.wagerLeft.ToString();
        changePriceText.text = "$" + GameManager.instance.gameData.changePrice.ToString();
        goldText.text = "Gold: $" + GameManager.instance.gameData.gold.ToString();
    }

    public void UpdateButtons(SlotMode mode)
    {
        switch (mode)
        {
            case SlotMode.ReadyForSpin:
                changeButton.interactable = false;
                confirmButton.interactable = false;
                spinButton.interactable = true;
                increaseButton.interactable = true;
                decreaseButton.interactable = true;
                break;
            case SlotMode.ChangingSymbols:
            case SlotMode.AllDisabled:
                changeButton.interactable = false;
                confirmButton.interactable = false;
                spinButton.interactable = false;
                increaseButton.interactable = false;
                decreaseButton.interactable = false;
                break;
            case SlotMode.WaitingForConfirm:
                changeButton.interactable = true;
                confirmButton.interactable = true;
                spinButton.interactable = false;
                increaseButton.interactable = false;
                decreaseButton.interactable = false;
                break;
        }
        
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slotUIController = new SlotUIController();
        SetFinishButton(false);
        UpdateButtons(SlotMode.ReadyForSpin);
        UpdateUI();

    }

    public void CreateSymbolUI(GameObject slotSymbolPrefab, GameObject slotPanel,  string[,] slotGrid, int row, int col, float cellWidth, float cellHeight)
    {
        GameObject newSymbol = Instantiate(slotSymbolPrefab, slotPanel.transform);
        newSymbol.GetComponent<TMP_Text>().text = slotGrid[row, col];

        RectTransform rectTransform = newSymbol.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);
        rectTransform.localScale = Vector3.one;

        int r = row, c = col;
        newSymbol.GetComponent<Button>().onClick.AddListener(() => ChangeSymbol(r, c));
    }
    
    public void FinishRound()
    {
        GameManager.instance.slotUIManager.UpdateInstructionText("You Win!");
        GameManager.instance.slotUIManager.pauseManager.winLoseMenu.SetActive(true);
        GameManager.instance.slotUIManager.pauseManager.restartButton.gameObject.SetActive(false);
        GameManager.instance.gameData.currentLevel++;
        GameManager.instance.gameData.targetMoney = (int)GameManager.instance.gameData.baseMoney * (int)(GameManager.instance.gameData.currentLevel*GameManager.instance.gameData.currentLevel*0.5);
        GameManager.instance.gameData.gold += 5 + GameManager.instance.gameData.currentLevel*2;
        GameManager.instance.SaveGame();
        GameManager.instance.slotUIManager.UpdateButtons(SlotMode.AllDisabled);
    }
    
    public void Spin()
    {
        if (GameManager.instance.gameData.spinsLeft == 0)
        {
            GameManager.instance.slotUIManager.UpdateInstructionText("You don't have spins left!");
            return;
        }
        GameManager.instance.slotUIManager.UpdateButtons(SlotMode.WaitingForConfirm);
        
        if (GameManager.instance.gameData.spinsLeft > 0 && GameManager.instance.gameData.money >= GameManager.instance.gameData.betAmount && GameManager.instance.gameData.wagerLeft >= GameManager.instance.gameData.betAmount)
        {
            GameManager.instance.gameData.spinsLeft--;
            GameManager.instance.gameData.money -= GameManager.instance.gameData.betAmount;
            GameManager.instance.gameData.wagerLeft -= GameManager.instance.gameData.betAmount;
            currentWin = GameManager.instance.slotMachine.SpinSlot(GameManager.instance.gameData.betAmount, false) * winCoef;

            GameManager.instance.slotUIManager.UpdateResultText("Current win: $" + currentWin);
        }
        else if (GameManager.instance.gameData.spinsLeft <= 0)
        {
            GameManager.instance.slotUIManager.UpdateResultText("Not enough spins!");
        }
        else if (GameManager.instance.gameData.wagerLeft < GameManager.instance.gameData.betAmount)
        {
            GameManager.instance.slotUIManager.UpdateResultText("Not enough wager!");
        }
        else
        {
            GameManager.instance.slotUIManager.UpdateResultText("Not enough balance!");
        }
    }
    public void ConfirmSpin()
    {
        GameManager.instance.slotUIManager.UpdateButtons(SlotMode.ReadyForSpin);

        GameManager.instance.gameData.money += currentWin;
        GameManager.instance.slotUIManager.UpdateResultText("You won: $" + currentWin);
        GameManager.instance.slotUIManager.UpdateInstructionText("");
        slotUIController.CheckLevelEnd(betAmounts);
    }
    
    public void IncreaseBet()
    {
        if (GameManager.instance.gameData.money > GameManager.instance.gameData.betAmount && GameManager.instance.gameData.betAmount < betAmounts[^1])
        {
            GameManager.instance.gameData.betAmount = betAmounts[betAmounts.FindIndex(x => x == GameManager.instance.gameData.betAmount) + 1];
            GameManager.instance.gameData.changePrice = GameManager.instance.gameData.betAmount / 2;
            GameManager.instance.slotUIManager.UpdateUI();
        }
    }

    public void DecreaseBet()
    {
        if (GameManager.instance.gameData.betAmount > betAmounts[0])
        {
            GameManager.instance.gameData.betAmount = betAmounts[betAmounts.FindIndex(x => x == GameManager.instance.gameData.betAmount) - 1];
            GameManager.instance.gameData.changePrice = GameManager.instance.gameData.betAmount / 2;
            GameManager.instance.slotUIManager.UpdateUI();
        }
    }

    public void StartChangingSymbol()
    {
        if (GameManager.instance.gameData.changePrice > GameManager.instance.gameData.money)
        {
            GameManager.instance.slotUIManager.UpdateInstructionText("Not enough money to change symbol!");
            return;
        }
        
        GameManager.instance.gameData.money -= GameManager.instance.gameData.changePrice;
        isChangingSymbol = true;
        
        GameManager.instance.slotUIManager.UpdateButtons(SlotMode.ChangingSymbols);

        GameManager.instance.slotUIManager.UpdateInstructionText("Select a symbol to change!");
    }
    public void SetFinishButton(bool interactable)
    {
        finishRoundButton.interactable = interactable;
        finishRoundText.color = new Color(0, 0, 0, interactable ?  1f: 0.4f);
    }

    public void UpdateResultText(string result)
    {
        resultText.text = result;
        UpdateUI();

    }
    public void ChangeSymbol(int row, int col)
    {
        if (!isChangingSymbol) return;
        isChangingSymbol = false;
        GameManager.instance.slotMachine.RandomizeSymbol(row, col);
        currentWin = slotUIController.FinishChangingSymbol();
    }
    
    public void UpdateInstructionText(string result)
    {
        instructionText.text = result;
        UpdateUI();

    }
}
    
