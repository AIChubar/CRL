using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]private GameData gameData;
    
    [SerializeField]private GameData newGameData;
    
    public static GameManager instance { get; private set; }
    public EventManager eventManager;
    [Header("Saving File name")] 
    [SerializeField] public string fileName;
    public DataPersistenceManager dataPersistenceManager;
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        eventManager = new EventManager();
        dataPersistenceManager = new DataPersistenceManager(fileName, gameData, newGameData);
    }


}
