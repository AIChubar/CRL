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

public class SlotControls : MonoBehaviour
{

    
    public TMP_Text moneyText;
    public TMP_Text targetText;
    public TMP_Text wagerText;
    public TMP_Text spinsText;
    public TMP_Text betText;
    public TMP_Text resultText;
    public TMP_Text instructionText;
    public TMP_Text changePriceText;

    public Button changeButton;
    public Button confirmButton;
    public Button spinButton;
    public Button increaseButton;
    public Button decreaseButton;

    void Awake()
    {
    }
    
    public void UpdateUI()
    {
        moneyText.text = "Money: $" + GameManager.instance.gameData.money.ToString("0.00");
        spinsText.text = "Spins Left: " + GameManager.instance.gameData.spinsLeft.ToString();
        betText.text = "Bet: $" + GameManager.instance.gameData.betAmount.ToString();
        targetText.text = "Target Money: $" + GameManager.instance.gameData.targetMoney.ToString("0.00");
        wagerText.text = "Wager Left: $" + GameManager.instance.gameData.wagerLeft.ToString();
        changePriceText.text = "$" + GameManager.instance.gameData.changePrice.ToString();
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
        UpdateButtons(SlotMode.ReadyForSpin);
        UpdateUI();

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
    
