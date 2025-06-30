using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that manages saving and loading data.
/// </summary>
public class DataPersistenceManager
{
    private GameData gameData;
    
    private GameData newGameData;
    
    private FileDataHandler dataHandler;

    private string fileName;

    public DataPersistenceManager(string fileName, GameData gameData, GameData newGameData)
    {
        this.gameData = gameData;
        this.newGameData = newGameData;
        this.fileName = fileName;
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }
    //private List<IDataPersistence> dataPersistenceObjects;
    public void NewGame()
    {
        gameData.CopyFrom(newGameData);
        gameData.Reset();
        gameData.shopConfig.Reset();
        SaveGame();
    }

    public void LoadGame()
    {
        if (gameData == null)
        {
            Debug.LogError("GameData instance is not assigned!");
            return;
        }
        
        dataHandler.Load(gameData);
        
    }


    public void SaveGame()
    {
        if (gameData == null)
        {
            return;
        }
        
        dataHandler.Save(gameData);
    }

   

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
