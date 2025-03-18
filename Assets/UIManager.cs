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
    
    public void OnRerollButtonPressed()
    {
        ShopManager.instance.RerollItems();
    }
    
    public void OnBuyButtonPressed()
    {
        ShopManager.instance.BuyItem();
    }
}
