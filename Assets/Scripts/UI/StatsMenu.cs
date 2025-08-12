using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI payoutBonusText;
    [SerializeField] private TextMeshProUGUI payoutMultiplierText;
    [SerializeField] private TextMeshProUGUI wildLuckText;
    
    [SerializeField] private Button statsMenuButton;
    private SlotUIManager slotUIManager;
    private GameData gameData;
    
    public void SetUp(GameData gd, SlotUIManager sm)
    {
        slotUIManager = sm;
        gameData = gd;
        statsMenuButton.onClick.AddListener(ShowStatsMenu);
    }

    private void ShowStatsMenu()
    {
        UpdateUI();
        this.gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public bool IsOpen => gameObject.activeSelf;


    private void UpdateUI()
    {
        payoutBonusText.text = "Payout Bonus: " + gameData.payoutBonus.Value;
        payoutMultiplierText.text = "Payout Multiplier: " + gameData.payoutMult.Value;
        wildLuckText.text = "Wild Luck: " + gameData.wildLuck.Value;
    }
}
  