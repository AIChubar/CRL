using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that manages saving and loading data.
/// </summary>
public class DataPersistanceManager : MonoBehaviour
{
    [Header("File Storage Config")] 
    [SerializeField] public string fileName;
    
    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    public FileDataHandler dataHandler;
    public static DataPersistanceManager instance { get; private set; }

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

    }

    public FileDataHandler GetNewDataHandler()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        return dataHandler;
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.NewGame(gameData);
        }
    }

    public void LoadGame()
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        GameData loadedGameData = dataHandler.Load();

        if (loadedGameData == null)
            return;

        gameData = loadedGameData;
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }



    public void SaveGame()
    {
        if (gameData == null)
        {
            return;
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }



    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        if (SceneManager.GetActiveScene().buildIndex == 0)
            LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        
    }

    /*private void OnApplicationQuit()
    {
        SaveGame();
    }*/

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects); 
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
