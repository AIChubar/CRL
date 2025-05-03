using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlotUIController
{
    private GameData gameData;
    private SlotMachine slotMachine;
    private SlotUIManager slotUIManager;
    private float currentWin = 0;

    public void SetUp(GameData gameData, SlotMachine slotMachine, SlotUIManager slotUIManager)
    {
        this.gameData = gameData;
        this.slotMachine = slotMachine;
        this.slotUIManager = slotUIManager;
    }

    public void Spin()
    {
        if (gameData.spinsLeft == 0)
        {
            slotUIManager.UpdateInstructionText("You don't have spins left!");
            return;
        }
        slotUIManager.slotMode = SlotMode.WaitingForConfirm;
        slotUIManager.UpdateButtons();

        if (gameData.spinsLeft > 0 /* && gameData.money >= gameData.betAmount && gameData.wagerLeft >= gameData.betAmount*/)
        {
            gameData.spinsLeft--;
            //gameData.money -= gameData.betAmount;
            //gameData.wagerLeft -= gameData.betAmount;
            currentWin = slotMachine.SpinSlot(gameData.betAmount, false) * gameData.payoutMult.Value + gameData.payoutBonus.Value;
            slotUIManager.UpdateResultText($"Current win: ${currentWin}");
        }
        else
        {
            slotUIManager.UpdateResultText("Not enough spins!");
        }
    }

    public void ConfirmSpin()
    {
        slotUIManager.slotMode = SlotMode.ReadyForSpin;
        slotUIManager.UpdateButtons();
        gameData.money += currentWin;
        slotUIManager.UpdateResultText($"You won: ${currentWin}");
        slotUIManager.UpdateInstructionText("");
        CheckLevelEnd();
    }

    public void OnChangeButtonClick()
    {
        if (gameData.changePrice > gameData.tokens)
        {
            slotUIManager.UpdateInstructionText("Not enough money to change symbol!");
            return;
        }
        gameData.tokens -= gameData.changePrice;

        slotUIManager.StartChangingSymbol();
    }
    
    

    public void ChangeSymbol(int row, int col)
    {

        slotUIManager.slotMode = SlotMode.WaitingForConfirm;
        slotUIManager.UpdateButtons();
        currentWin = slotMachine.RandomizeSymbolCalculate(row, col) * gameData.payoutMult.Value + gameData.payoutBonus.Value;
        slotUIManager.UpdateResultText($"Current win: ${currentWin}");
        slotUIManager.UpdateInstructionText("");
        
    }
    
    public void ChangeColumn(int col)
    {
        slotUIManager.slotMode = SlotMode.WaitingForConfirm;
        slotUIManager.UpdateButtons();
        currentWin = slotMachine.RandomizeColumnCalculate( col) * gameData.payoutMult.Value + gameData.payoutBonus.Value;
        slotUIManager.UpdateResultText($"Current win: ${currentWin}");
        slotUIManager.UpdateInstructionText("");
    }

    

    public void IncreaseBet()
    {
        int currentIndex = slotUIManager.betAmounts.IndexOf(gameData.betAmount);
        if (currentIndex < slotUIManager.betAmounts.Count - 1 && gameData.money > slotUIManager.betAmounts[currentIndex])
        {
            gameData.betAmount = slotUIManager.betAmounts[currentIndex + 1];
            slotUIManager.UpdateUI();
        }
    }

    public void DecreaseBet()
    {
        int currentIndex = slotUIManager.betAmounts.IndexOf(gameData.betAmount);
        if (currentIndex > 0)
        {
            gameData.betAmount = slotUIManager.betAmounts[currentIndex - 1];
            slotUIManager.UpdateUI();
        }
    }

    public void CheckLevelEnd()
    {
        if (gameData.money >= gameData.targetMoney)
        {
            slotUIManager.SetFinishButton(true);
        }
        else if (gameData.spinsLeft == 0 /*|| gameData.wagerLeft < slotUIManager.betAmounts[0] || gameData.money < slotUIManager.betAmounts[0]*/)
        {
            slotUIManager.UpdateInstructionText("You Lose!");
            slotUIManager.pauseManager.winLoseMenu.SetActive(true);
            slotUIManager.pauseManager.nextLevelButton.gameObject.SetActive(false);
            slotUIManager.DisableButtons();
        }
        else
        {
            slotUIManager.SetFinishButton(false);
        }
    }
    
    public void SaveGame()
    {
        GameManager.instance.dataPersistenceManager.SaveGame();
    }
    
    public void LoadGame()
    {
        GameManager.instance.dataPersistenceManager.LoadGame();
    }
    
    public void Restart()
    {
        slotUIManager.pauseManager.winLoseMenu.SetActive(false);
        LoadGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void FinishRound()
    {
        int income = (int)(gameData.money * 0.1f);
        int spinsLeft = gameData.spinsLeft;
        int reward = gameData.currentLevel * 2;
        int goldReceived = income + reward + spinsLeft;
        slotUIManager.UpdateInstructionText($"You Win! Gold Received : {goldReceived}");
        slotUIManager.pauseManager.winLoseMenu.SetActive(true);
        slotUIManager.pauseManager.restartButton.gameObject.SetActive(false);

        gameData.currentLevel++;
        gameData.targetMoney += 10*gameData.currentLevel;
        gameData.gold += goldReceived;
        gameData.Reset();

        SaveGame();
        slotUIManager.DisableButtons();
    }

    
    
    public void Continue()
    {
        slotUIManager.pauseManager.PauseGame();
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void ToShop()
    {
        //SaveGame();
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

   
}


