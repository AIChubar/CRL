using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    // Function to handle the Spin button press
    public void OnSpinButtonPressed()
    {
        gameManager.Spin();  // Call the Spin function in GameManager
    }

    // Function to handle the Increase Bet button press
    public void OnIncreaseBetButtonPressed()
    {
        gameManager.IncreaseBet();  // Increase the bet amount
    }

    // Function to handle the Decrease Bet button press
    public void OnDecreaseBetButtonPressed()
    {
        gameManager.DecreaseBet();  // Decrease the bet amount
    }
    
    public void OnStartChangingButtonPressed()
    {
        gameManager.StartChangingSymbol();  // Decrease the bet amount
    }
    
    public void OnConfirmButtonPressed()
    {
        gameManager.ConfirmSpin();  // Decrease the bet amount
    }
    
    public void OnRestartButtonPressed()
    {
        gameManager.Restart();  // Decrease the bet amount
    }
    
    public void OnNextLevelButtonPressed()
    {
        gameManager.NextLevel();  // Decrease the bet amount
    }
}
