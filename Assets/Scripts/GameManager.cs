using UnityEngine;



public class GameManager : MonoBehaviour
{
    [HideInInspector]public static GameManager instance;
  
    [SerializeField]private SlotUIManager slotUIManager;
    
    public GameData gameData;
    
    public SlotMachine slotMachine;
    private SymbolManager symbolManager;
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
        slotMachine.SetUp(slotCalculator, slotUIManager, gameData);
    }
}