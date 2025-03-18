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
    
    
    
    [SerializeField]public SlotUIManager slotUIManager;
    
    public GameData gameData;
    
    public SlotMachine slotMachine;



    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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