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
    
    [HideInInspector]public SaveManager saveManager;
    [HideInInspector]public SlotControls slotControls;
    
    


    public PauseManager pauseManager;
    public SlotMachine slotMachine;
    public bool isChangingSymbol = false; 

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
            slotControls = FindFirstObjectByType<SlotControls>(); //Bad

        }
        else
        {
            Destroy(gameObject);
        }
    }
    
   
    void Start()
    {
        currentWin = saveManager.gameData.betAmount / 2;
    }
    
    public void SaveGame()
    {
        DataPersistanceManager.instance.SaveGame();
    }
    
    public void LoadGame()
    {
        DataPersistanceManager.instance.LoadGame();
    }

    

    

    public void Spin()
    {
        slotControls.UpdateButtons(SlotMode.WaitingForConfirm);
        
        if (saveManager.gameData.spinsLeft > 0 && saveManager.gameData.money >= saveManager.gameData.betAmount && saveManager.gameData.wagerLeft >= saveManager.gameData.betAmount)
        {
            saveManager.gameData.spinsLeft--;
            saveManager.gameData.money -= saveManager.gameData.betAmount;
            saveManager.gameData.wagerLeft -= saveManager.gameData.betAmount;
            currentWin = slotMachine.SpinSlot(saveManager.gameData.betAmount, false) * winCoef;

            slotControls.UpdateResultText("Current win: $" + currentWin);
        }
        else if (saveManager.gameData.spinsLeft <= 0)
        {
            slotControls.UpdateResultText("Not enough spins!");
        }
        else if (saveManager.gameData.wagerLeft < saveManager.gameData.betAmount)
        {
            slotControls.UpdateResultText("Not enough wager!");
        }
        else
        {
            slotControls.UpdateResultText("Not enough balance!");
        }
    }

    public void IncreaseBet()
    {
        if (saveManager.gameData.money > saveManager.gameData.betAmount && saveManager.gameData.betAmount < betAmounts[^1])
        {
            saveManager.gameData.betAmount = betAmounts[betAmounts.FindIndex(x => x == saveManager.gameData.betAmount) + 1];
            saveManager.gameData.changePrice = saveManager.gameData.betAmount / 2;
            slotControls.UpdateUI();
        }
    }

    public void DecreaseBet()
    {
        if (saveManager.gameData.betAmount > betAmounts[0])
        {
            saveManager.gameData.betAmount = betAmounts[betAmounts.FindIndex(x => x == saveManager.gameData.betAmount) - 1];
            saveManager.gameData.changePrice = saveManager.gameData.betAmount / 2;
            slotControls.UpdateUI();
        }
    }

    

    public void StartChangingSymbol()
    {
        if (saveManager.gameData.changePrice > saveManager.gameData.money)
        {
            slotControls.UpdateInstructionText("Not enough money to change symbol!");
            return;
        }
        
        saveManager.gameData.money -= saveManager.gameData.changePrice;
        isChangingSymbol = true;
        
        slotControls.UpdateButtons(SlotMode.ChangingSymbols);

        slotControls.UpdateInstructionText("Select a symbol to change!");
    }
    
    public void FinishChangingSymbol()
    {
        slotControls.UpdateButtons(SlotMode.WaitingForConfirm);

        currentWin = slotMachine.CalculateWin(saveManager.gameData.betAmount) * winCoef;
        slotControls.UpdateResultText("Current win: $" + currentWin);
        slotControls.UpdateInstructionText("");
    }

    public void ConfirmSpin()
    {
        slotControls.UpdateButtons(SlotMode.ReadyForSpin);

        saveManager.gameData.money += currentWin;
        slotControls.UpdateResultText("You won: $" + currentWin);
        slotControls.UpdateInstructionText("");
        CheckLevelEnd();
    }

    private void CheckLevelEnd()
    {
        if (saveManager.gameData.money >= saveManager.gameData.targetMoney)
        {
            slotControls.UpdateInstructionText("You Win!");
            pauseManager.winLoseMenu.SetActive(true);
            pauseManager.restartButton.gameObject.SetActive(false);
            saveManager.gameData.currentLevel++;
            saveManager.gameData.targetMoney = saveManager.gameData.targetMoney * 2 + (int)saveManager.gameData.money;
            SaveGame();
            slotControls.UpdateButtons(SlotMode.AllDisabled);
        }
        else if (saveManager.gameData.spinsLeft == 0 || saveManager.gameData.wagerLeft < betAmounts[0] || saveManager.gameData.money < betAmounts[0])
        {
            slotControls.UpdateInstructionText("You Lose!");
            pauseManager.winLoseMenu.SetActive(true);
            pauseManager.nextLevelButton.gameObject.SetActive(false);
            slotControls.UpdateButtons(SlotMode.AllDisabled);

        }
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
        pauseManager.winLoseMenu.SetActive(false);
        LoadGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        pauseManager.winLoseMenu.SetActive(false);
        LoadGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void Continue()
    {
        pauseManager.PauseGame();
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}