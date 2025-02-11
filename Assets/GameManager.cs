using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public List<int> betAmounts =  new List<int>(){ 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 140, 160, 180, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000 };
    
    
    private float currentWin = 0;
    
     public SaveManager saveManager;
    
    public TMP_Text moneyText;
    public TMP_Text changePriceText;
    public TMP_Text targetText;
    public TMP_Text wagerText;
    public TMP_Text spinsText;
    public TMP_Text betText;
    public TMP_Text resultText;
    public TMP_Text instructionText;

    public GameObject WinLoseMenu;
    
    public Button RestartButton;
    public Button NextLevelButton;
    
    public Button changeButton;
    public Button confirmButton;
    public Button spinButton;
    public Button increaseButton;
    public Button decreaseButton;

    
    
    public SlotMachine slotMachine;
    public bool isChangingSymbol = false; // Флаг изменения символа

    [Header("Monte Carlo Simulation Results")]
    public int simulationRuns = 1000;
    public float averageWin;
    public float winProbability;
    public float winCoef;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            saveManager = FindFirstObjectByType<SaveManager>(); //Bad
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
   
    void Start()
    {
        WinLoseMenu.SetActive(false);
        currentWin = saveManager.gameData.betAmount / 2;
        changeButton.interactable = false;
        confirmButton.interactable = false;
        spinButton.interactable = true;
        increaseButton.interactable = true;
        decreaseButton.interactable = true;
        
        UpdateUI();
    }
    
    public void SaveGame()
    {
        DataPersistanceManager.instance.SaveGame();
    }
    
    public void LoadGame()
    {
    }

    void UpdateUI()
    {
        moneyText.text = "Money: $" + saveManager.gameData.money.ToString("0.00");
        spinsText.text = "Spins Left: " + saveManager.gameData.spinsLeft.ToString();
        betText.text = "Bet: $" + saveManager.gameData.betAmount.ToString();
        targetText.text = "Target Money: $" + saveManager.gameData.targetMoney.ToString("0.00");
        wagerText.text = "Wager Left: $" + saveManager.gameData.wagerLeft.ToString();
        changePriceText.text = "$" + saveManager.gameData.changePrice.ToString();
    }

    public void Spin()
    {
        changeButton.interactable = true;
        confirmButton.interactable = true;
        spinButton.interactable = false;
        increaseButton.interactable = false;
        decreaseButton.interactable = false;
        if (saveManager.gameData.spinsLeft > 0 && saveManager.gameData.money >= saveManager.gameData.betAmount && saveManager.gameData.wagerLeft >= saveManager.gameData.betAmount)
        {
            saveManager.gameData.spinsLeft--;
            saveManager.gameData.money -= saveManager.gameData.betAmount;
            saveManager.gameData.wagerLeft -= saveManager.gameData.betAmount;
            currentWin = slotMachine.SpinSlot(saveManager.gameData.betAmount, false) * winCoef;


            resultText.text = "Current win: $" + currentWin;
            UpdateUI();
        }
        else if (saveManager.gameData.spinsLeft <= 0)
        {
            resultText.text = "Not enough spins!";
        }
        else if (saveManager.gameData.wagerLeft < saveManager.gameData.betAmount)
        {
            resultText.text = "Not enough wager!";
        }
        else
        {
            resultText.text = "Not enough balance!";
        }
    }

    public void IncreaseBet()
    {
        if (saveManager.gameData.money > saveManager.gameData.betAmount && saveManager.gameData.betAmount < betAmounts[^1])
        {
            saveManager.gameData.betAmount = betAmounts[betAmounts.FindIndex(x => x == saveManager.gameData.betAmount) + 1];
            saveManager.gameData.changePrice = saveManager.gameData.betAmount / 2;
            UpdateUI();
        }
    }

    public void DecreaseBet()
    {
        if (saveManager.gameData.betAmount > betAmounts[0])
        {
            saveManager.gameData.betAmount = betAmounts[betAmounts.FindIndex(x => x == saveManager.gameData.betAmount) - 1];
            saveManager.gameData.changePrice = saveManager.gameData.betAmount / 2;
            UpdateUI();
        }
    }

    

    public void StartChangingSymbol()
    {
        if (saveManager.gameData.changePrice > saveManager.gameData.money)
        {
            instructionText.text = "Not enough money to change symbol!";
            return;
        }
        
        saveManager.gameData.money -= saveManager.gameData.changePrice;
        isChangingSymbol = true;
        
        changeButton.interactable = false;
        confirmButton.interactable = false;
        spinButton.interactable = false;
        increaseButton.interactable = false;
        decreaseButton.interactable = false;
        instructionText.text = "Select a symbol to change!";
        UpdateUI();
    }
    
    public void FinishChangingSymbol()
    {
        changeButton.interactable = true;
        confirmButton.interactable = true;
        spinButton.interactable = false;
        increaseButton.interactable = false;
        decreaseButton.interactable = false;
        currentWin = slotMachine.CalculateWin(saveManager.gameData.betAmount) * winCoef;
        resultText.text = "Current win: $" + currentWin;
        instructionText.text = "";
        UpdateUI();
    }

    public void ConfirmSpin()
    {
        changeButton.interactable = false;
        confirmButton.interactable = false;
        spinButton.interactable = true;
        increaseButton.interactable = true;
        decreaseButton.interactable = true;
        saveManager.gameData.money += currentWin;
        resultText.text = "You won: $" + currentWin;
        UpdateUI();
        CheckLevelEnd();
    }

    private void CheckLevelEnd()
    {
        if (saveManager.gameData.money >= saveManager.gameData.targetMoney)
        {
            instructionText.text = "You Win!";
            WinLoseMenu.SetActive(true);
            RestartButton.gameObject.SetActive(false);
            saveManager.gameData.currentLevel++;
            saveManager.gameData.targetMoney = saveManager.gameData.targetMoney * 2 + (int)saveManager.gameData.money;
            SaveGame();
            DisableAllButtons();
        }
        else if (saveManager.gameData.spinsLeft == 0 || saveManager.gameData.wagerLeft < betAmounts[0] || saveManager.gameData.money < betAmounts[0])
        {
            instructionText.text = "You lose!";
            WinLoseMenu.SetActive(true);
            NextLevelButton.gameObject.SetActive(false);
            DisableAllButtons();
        }
    }

    public void DisableAllButtons()
    {
        changeButton.interactable = false;
        confirmButton.interactable = false;
        spinButton.interactable = false;
        increaseButton.interactable = false;
        decreaseButton.interactable = false;
    }
    
    public void RunMonteCarloSimulation()
    {
        int totalWin = 0;
        int totalWins = 0;

        for (int i = 0; i < simulationRuns; i++)
        {
            int result = slotMachine.SpinSlot(saveManager.gameData.betAmount, true);
            totalWin += result;

            if (result > 0)
            {
                totalWins++;
            }
        }

        averageWin = (float)Math.Round((float)totalWin / simulationRuns, 2 );
        winProbability = (float)Math.Round((float)totalWins / simulationRuns * 100, 2);
        winCoef = (float)Math.Round(saveManager.gameData.RTP * saveManager.gameData.betAmount / averageWin, 2 );
    }


    public void Restart()
    {
        LoadGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        WinLoseMenu.SetActive(false);
        LoadGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}