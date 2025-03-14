using UnityEngine;

public class SaveManager : MonoBehaviour, IDataPersistence
{
    public static SaveManager instance;
    [SerializeField]GameData gameData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData(GameData data)
    {
        gameData = data;
        if (GameManager.instance != null)
            GameManager.instance.gameData = data;
    }

    public void SaveData(ref GameData data)
    {
        if (GameManager.instance != null)
            gameData = GameManager.instance.gameData;
        data = gameData;
        
    }

    public void NewGame(GameData data)
    {
        gameData = data;
        if (GameManager.instance != null)
            GameManager.instance.gameData = data;
    }
}
