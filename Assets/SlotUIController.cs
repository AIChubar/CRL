using System.Collections.Generic;
using UnityEngine;

public class SlotUIController
{
    public SlotUIController()
    {

    }

    
    
    public float FinishChangingSymbol()
    {
        GameManager.instance.slotUIManager.UpdateButtons(SlotMode.WaitingForConfirm);

        float currentWin = GameManager.instance.slotMachine.CalculateWin(GameManager.instance.gameData.betAmount) /* winCoef*/;
        GameManager.instance.slotUIManager.UpdateResultText("Current win: $" + currentWin);
        GameManager.instance.slotUIManager.UpdateInstructionText("");
        return currentWin;
    }

  
    
    public void CheckLevelEnd(List<int> betAmounts)
    {
        if (GameManager.instance.gameData.money >= GameManager.instance.gameData.targetMoney)
        {
            GameManager.instance.slotUIManager.SetFinishButton(true);

        }
        else if (GameManager.instance.gameData.spinsLeft == 0 || GameManager.instance.gameData.wagerLeft < betAmounts[0] || GameManager.instance.gameData.money < betAmounts[0])
        {
            GameManager.instance.slotUIManager.UpdateInstructionText("You Lose!");
            GameManager.instance.slotUIManager.pauseManager.winLoseMenu.SetActive(true);
            GameManager.instance.slotUIManager.pauseManager.nextLevelButton.gameObject.SetActive(false);
            GameManager.instance.slotUIManager.UpdateButtons(SlotMode.AllDisabled);
        }
        else if(GameManager.instance.gameData.money < GameManager.instance.gameData.targetMoney)
        {
            GameManager.instance.slotUIManager.SetFinishButton(false);

        }
    }
    
    
    
    
}
