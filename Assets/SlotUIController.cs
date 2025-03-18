using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlotUIController
{
    private GameData gameData;
    private SlotMachine slotMachine;
    private SlotUIManager slotUIManager;
    private float currentWin = 0;

    public void Setup(GameData gameData, SlotMachine slotMachine, SlotUIManager slotUIManager)
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

        slotUIManager.UpdateButtons(SlotMode.WaitingForConfirm);

        if (gameData.spinsLeft > 0 && gameData.money >= gameData.betAmount && gameData.wagerLeft >= gameData.betAmount)
        {
            gameData.spinsLeft--;
            gameData.money -= gameData.betAmount;
            gameData.wagerLeft -= gameData.betAmount;
            currentWin = slotMachine.SpinSlot(gameData.betAmount, false) * slotUIManager.winCoef;

            slotUIManager.UpdateResultText($"Current win: ${currentWin}");
        }
        else
        {
            slotUIManager.UpdateResultText("Not enough resources!");
        }
    }

    public void ConfirmSpin()
    {
        slotUIManager.UpdateButtons(SlotMode.ReadyForSpin);
        gameData.money += currentWin;
        slotUIManager.UpdateResultText($"You won: ${currentWin}");
        slotUIManager.UpdateInstructionText("");
        CheckLevelEnd();
    }

    public void ChangeSymbol(int row, int col)
    {
        if (!slotUIManager.isChangingSymbol) return;

        slotUIManager.isChangingSymbol = false;
        slotMachine.RandomizeSymbol(row, col);
        currentWin = FinishChangingSymbol();
    }

    public float FinishChangingSymbol()
    {
        slotUIManager.UpdateButtons(SlotMode.WaitingForConfirm);
        float newWin = slotMachine.CalculateWin(gameData.betAmount);
        slotUIManager.UpdateResultText($"Current win: ${newWin}");
        return newWin;
    }

    public void IncreaseBet()
    {
        int currentIndex = slotUIManager.betAmounts.IndexOf(gameData.betAmount);
        if (currentIndex < slotUIManager.betAmounts.Count - 1 && gameData.money > slotUIManager.betAmounts[currentIndex])
        {
            gameData.betAmount = slotUIManager.betAmounts[currentIndex + 1];
            gameData.changePrice = gameData.betAmount / 2;
            slotUIManager.UpdateUI();
        }
    }

    public void DecreaseBet()
    {
        int currentIndex = slotUIManager.betAmounts.IndexOf(gameData.betAmount);
        if (currentIndex > 0)
        {
            gameData.betAmount = slotUIManager.betAmounts[currentIndex - 1];
            gameData.changePrice = gameData.betAmount / 2;
            slotUIManager.UpdateUI();
        }
    }

    public void CheckLevelEnd()
    {
        if (gameData.money >= gameData.targetMoney)
        {
            slotUIManager.SetFinishButton(true);
        }
        else if (gameData.spinsLeft == 0 || gameData.wagerLeft < slotUIManager.betAmounts[0] || gameData.money < slotUIManager.betAmounts[0])
        {
            slotUIManager.UpdateInstructionText("You Lose!");
            slotUIManager.pauseManager.winLoseMenu.SetActive(true);
            slotUIManager.pauseManager.nextLevelButton.gameObject.SetActive(false);
            slotUIManager.UpdateButtons(SlotMode.AllDisabled);
        }
        else
        {
            slotUIManager.SetFinishButton(false);
        }
    }
    
    public void SaveGame()
    {
        DataPersistanceManager.instance.SaveGame();
    }
    
    public void LoadGame()
    {
        DataPersistanceManager.instance.LoadGame();
    }
    
    public void Restart()
    {
        slotUIManager.pauseManager.winLoseMenu.SetActive(false);
        LoadGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void FinishRound()
    {
        slotUIManager.UpdateInstructionText("You Win!");
        slotUIManager.pauseManager.winLoseMenu.SetActive(true);
        slotUIManager.pauseManager.restartButton.gameObject.SetActive(false);

        gameData.currentLevel++;
        gameData.targetMoney = (int)(gameData.baseMoney * (gameData.currentLevel * gameData.currentLevel * 0.5));
        gameData.gold += 5 + gameData.currentLevel * 2;

        SaveGame();
        slotUIManager.UpdateButtons(SlotMode.AllDisabled);
    }

    public void NextLevel()
    {
        LoadGame();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        gameData.money = gameData.baseMoney;
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
        SaveGame();
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }
}


