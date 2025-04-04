using UnityEngine;



public class SlotSceneManager : MonoBehaviour
{
    //[HideInInspector]public static GameManager instance;
  
    [SerializeField]private SlotUIManager slotUIManager;
    
    public GameData gameData;
    
    private SlotMachine slotMachine;
    private SymbolManager symbolManager;
    private SlotCalculator slotCalculator;
    private void Start()
    {
        slotCalculator = new SlotCalculator();
        slotMachine = new SlotMachine();
        slotMachine.SetUp(slotCalculator, slotUIManager, gameData);
    }
}