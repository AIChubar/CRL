using UnityEngine;



public class GameManager : MonoBehaviour
{
    [HideInInspector]public static GameManager instance;
  
    [SerializeField]public SlotUIManager slotUIManager;
    
    public GameData gameData;
    
    public SlotMachine slotMachine;
    
    private SlotCalculator slotCalculator;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        slotCalculator = new SlotCalculator();
        
        slotUIManager.Setup(gameData, slotMachine);
        slotMachine.Setup(slotCalculator);
    }
}