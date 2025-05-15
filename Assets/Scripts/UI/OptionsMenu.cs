using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    
    [SerializeField]private GameData gameData;


    public void OnNormalSpeedSet()
    {
        gameData.animationSpeed = 1.0f;
    }
    
    public void OnFastSpeedSet()
    {
        gameData.animationSpeed = 0.5f;

    }
    
    public void OnInstantSpeedSet()
    {
        gameData.animationSpeed = 0.2f;

    }
}
