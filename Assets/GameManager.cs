using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [HideInInspector]public static GameManager instance;
    
    public List<int> betAmounts =  new List<int>(){ 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 140, 160, 180, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000 };
    
    private float currentWin = 0;
    
    [HideInInspector]public SlotControls slotControls;
    
    public GameData gameData;
    
    [SerializeField] private Button finishRoundButton;


    public PauseManager pauseManager;
    public SlotMachine slotMachine;
    public bool isChangingSymbol = false; 

    public float winCoef;


    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            slotControls = FindFirstObjectByType<SlotControls>(); //Bad
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
   
    void Start()
    {
        finishRoundButton.interactable = false;
        //finishRoundText.color = new Color(1, 1, 1, 0.4f);

        currentWin = gameData.betAmount / 2;
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
        if (gameData.spinsLeft == 0)
        {
            slotControls.UpdateInstructionText("You don't have spins left!");
            return;
        }
        slotControls.UpdateButtons(SlotMode.WaitingForConfirm);
        
        if (gameData.spinsLeft > 0 && gameData.money >= gameData.betAmount && gameData.wagerLeft >= gameData.betAmount)
        {
            gameData.spinsLeft--;
            gameData.money -= gameData.betAmount;
            gameData.wagerLeft -= gameData.betAmount;
            currentWin = slotMachine.SpinSlot(gameData.betAmount, false) * winCoef;

            slotControls.UpdateResultText("Current win: $" + currentWin);
        }
        else if (gameData.spinsLeft <= 0)
        {
            slotControls.UpdateResultText("Not enough spins!");
        }
        else if (gameData.wagerLeft < gameData.betAmount)
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
        if (gameData.money > gameData.betAmount && gameData.betAmount < betAmounts[^1])
        {
            gameData.betAmount = betAmounts[betAmounts.FindIndex(x => x == gameData.betAmount) + 1];
            gameData.changePrice = gameData.betAmount / 2;
            slotControls.UpdateUI();
        }
    }

    public void DecreaseBet()
    {
        if (gameData.betAmount > betAmounts[0])
        {
            gameData.betAmount = betAmounts[betAmounts.FindIndex(x => x == gameData.betAmount) - 1];
            gameData.changePrice = gameData.betAmount / 2;
            slotControls.UpdateUI();
        }
    }

    public void StartChangingSymbol()
    {
        if (gameData.changePrice > gameData.money)
        {
            slotControls.UpdateInstructionText("Not enough money to change symbol!");
            return;
        }
        
        gameData.money -= gameData.changePrice;
        isChangingSymbol = true;
        
        slotControls.UpdateButtons(SlotMode.ChangingSymbols);

        slotControls.UpdateInstructionText("Select a symbol to change!");
    }
    
    public void FinishChangingSymbol()
    {
        slotControls.UpdateButtons(SlotMode.WaitingForConfirm);

        currentWin = slotMachine.CalculateWin(gameData.betAmount) * winCoef;
        slotControls.UpdateResultText("Current win: $" + currentWin);
        slotControls.UpdateInstructionText("");
    }

    public void ConfirmSpin()
    {
        slotControls.UpdateButtons(SlotMode.ReadyForSpin);

        gameData.money += currentWin;
        slotControls.UpdateResultText("You won: $" + currentWin);
        slotControls.UpdateInstructionText("");
        CheckLevelEnd();
    }

    private void CheckLevelEnd()
    {
        if (gameData.money >= gameData.targetMoney)
        {
            finishRoundButton.interactable = true;
            //finishRoundText.color = new Color(1, 1, 1, 1);
        }
        else if (gameData.spinsLeft == 0 || gameData.wagerLeft < betAmounts[0] || gameData.money < betAmounts[0])
        {
            slotControls.UpdateInstructionText("You Lose!");
            pauseManager.winLoseMenu.SetActive(true);
            pauseManager.nextLevelButton.gameObject.SetActive(false);
            slotControls.UpdateButtons(SlotMode.AllDisabled);
        }
        else if(gameData.money < gameData.targetMoney)
        {
            finishRoundButton.interactable = false;
            //finishRoundText.color = new Color(1, 1, 1, 0.4f);
        }
    }

    public void FinishRound()
    {
        slotControls.UpdateInstructionText("You Win!");
        pauseManager.winLoseMenu.SetActive(true);
        pauseManager.restartButton.gameObject.SetActive(false);
        gameData.currentLevel++;
        gameData.targetMoney = (int)gameData.baseMoney * (int)(gameData.currentLevel*gameData.currentLevel*0.5);
        gameData.gold += 5 + gameData.currentLevel*2;
        SaveGame();
        slotControls.UpdateButtons(SlotMode.AllDisabled);
    }
    

    
    public void Restart()
    {
        pauseManager.winLoseMenu.SetActive(false);
        LoadGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        LoadGame();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        gameData.money = gameData.baseMoney;
    }

    public void Continue()
    {
        pauseManager.PauseGame();
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void ToShop()
    {
        SaveGame();
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }
}