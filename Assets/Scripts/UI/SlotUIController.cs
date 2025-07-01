using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlotUIController
{
    private GameData gameData;
    private SlotMachine slotMachine;
    private SlotUIManager slotUIManager;
    private SlotGridManager slotGridManager;
    private float currentWin = 0;

    public void SetUp(GameData gameData, SlotMachine slotMachine, SlotUIManager slotUIManager, SlotGridManager slotGridManager)
    {
        this.slotGridManager = slotGridManager;
        this.gameData = gameData;
        this.slotMachine = slotMachine;
        this.slotUIManager = slotUIManager;
        slotUIManager.UpdateResultText($"Current win: ${currentWin:0.00}");
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
            currentWin = (slotMachine.SpinSlot(gameData.betAmount) + gameData.payoutBonus.Value)  * gameData.payoutMult.Value;
            slotUIManager.UpdateResultText($"Current win: ${currentWin:0.00}");
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
        slotUIManager.UpdateResultText($"You won: ${currentWin:0.00}");
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
    
    public void OnWildButtonClick()
    {
        if (gameData.wildPrice > gameData.tokens)
        {
            slotUIManager.UpdateInstructionText("Not enough money to change symbol!");
            return;
        }
        gameData.tokens -= gameData.wildPrice;
        StatModifier wildLuckMax = ScriptableObject.CreateInstance<StatModifier>();
        wildLuckMax.Description = "Temporary wild guarantee";
        wildLuckMax.Value = 1000000f;
        wildLuckMax.StatModType = StatModType.PercentAdd;
        wildLuckMax.StatType = StatType.WildLuck;
        wildLuckMax.IsTemporary = true;
        wildLuckMax.Order = 0;
        gameData.wildLuck.AddModifier(wildLuckMax);
        slotUIManager.StartChangingSymbol();
    }

    public void ChangeSymbol(int row, int col)
    {
       
        slotUIManager.slotMode = SlotMode.WaitingForConfirm;
        slotUIManager.UpdateButtons();
        currentWin = slotMachine.RandomizeSymbolCalculate(row, col) * gameData.payoutMult.Value + gameData.payoutBonus.Value;
        slotUIManager.UpdateResultText($"Current win: ${currentWin:0.00}");
        slotUIManager.UpdateInstructionText("");
        gameData.wildLuck.RemoveTemporaryModifiers();
    }
    
    public void ChangeColumn(int col)
    {
        if (slotUIManager.slotMode == SlotMode.ChangingColumn)
        {
            slotUIManager.slotMode = SlotMode.WaitingForConfirm;
            slotUIManager.UpdateButtons();
            currentWin = slotMachine.RandomizeColumnCalculate( col) * gameData.payoutMult.Value + gameData.payoutBonus.Value;
            slotUIManager.UpdateResultText($"Current win: ${currentWin:0.00}");
            slotUIManager.UpdateInstructionText("");
        }
        else if (slotUIManager.slotMode == SlotMode.BuffingColumn)
        {
            slotUIManager.slotMode = SlotMode.WaitingForConfirm;
            slotUIManager.UpdateButtons();
            slotGridManager.BuffColumn(col,1);
            currentWin = slotMachine.RecalculateWithAnimation() * gameData.payoutMult.Value + gameData.payoutBonus.Value;
            slotUIManager.UpdateResultText($"Current win: ${currentWin:0.00}");
            slotUIManager.UpdateInstructionText("");
        }
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
        int reward = 5 + gameData.currentLevel;
        int goldReceived = income > 10 ? 10 : income + reward + spinsLeft;
        slotUIManager.UpdateInstructionText($"You Win! Gold Received : {goldReceived}");
        slotUIManager.pauseManager.winLoseMenu.SetActive(true);
        slotUIManager.pauseManager.restartButton.gameObject.SetActive(false);
        gameData.currentLevel++;
        if (gameData.levelsTargetMoney.Count > gameData.currentLevel)
            gameData.targetMoney = gameData.levelsTargetMoney[gameData.currentLevel];
        else
            gameData.targetMoney *= 2;
        gameData.gold += goldReceived;
        gameData.initialLevelTokens = gameData.tokens;
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


