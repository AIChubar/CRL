using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]private GameData gameData;
    
    [SerializeField]private GameData newGameData;
    
    public static GameManager instance { get; private set; }
    public EventManager eventManager;
    public RNGManager rngManager;

    [Header("Saving File name")] 
    [SerializeField] public string fileName;
    [Header("RNG Seed")] 
    [SerializeField] public int seed;
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
       
    }

    private void Start()
    {
        rngManager = new RNGManager();
        rngManager.SetUp(seed);
        eventManager = new EventManager();
        dataPersistenceManager = new DataPersistenceManager(fileName, gameData, newGameData);
    }

}
