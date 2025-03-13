using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;


    private void Start()
    {
        gameManager = GameManager.instance;
    }

    // Function to handle the Spin button press
    public void OnSpinButtonPressed()
    {
        gameManager.Spin();  // Call the Spin function in GameManager
    }

    // Function to handle the Increase Bet button press
    public void OnIncreaseBetButtonPressed()
    {
        gameManager.IncreaseBet();  // Increase the bet amount
    }

    // Function to handle the Decrease Bet button press
    public void OnDecreaseBetButtonPressed()
    {
        gameManager.DecreaseBet();  // Decrease the bet amount
    }
    
    public void OnStartChangingButtonPressed()
    {
        gameManager.StartChangingSymbol();  // Decrease the bet amount
    }
    
    public void OnConfirmButtonPressed()
    {
        gameManager.ConfirmSpin();  // Decrease the bet amount
    }
    
    public void OnRestartButtonPressed()
    {
        gameManager.Restart();  // Decrease the bet amount
    }
    
    public void OnNextLevelButtonPressed()
    {
        gameManager.NextLevel();  // Decrease the bet amount
    }
    
    public void OnToShopButtonPressed()
    {
        gameManager.ToShop();  // Decrease the bet amount
    }
    
    public void OnContinueButtonPressed()
    {
        gameManager.Continue();  // Decrease the bet amount
    }
    public void OnMenuButtonPressed()
    {
        gameManager.ToMenu();  // Decrease the bet amount
    }

    public void OnRerollButtonPressed()
    {
        ShopManager.instance.RerollItems();
    }
}
