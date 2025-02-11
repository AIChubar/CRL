using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    
    [SerializeField]
    private Button newGameButton;
    
    
    [SerializeField] private TextMeshProUGUI continueText;

    [SerializeField] 
    private Button continueGameButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (!DataPersistanceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false;
            continueText.color = new Color(1, 1, 1, 0.4f);
        }
    }
    
    public void OnNewGameClicked()
    {
        DataPersistanceManager.instance.NewGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void OnContinueGameClicked()
    {
        DataPersistanceManager.instance.LoadGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnQuitGameClicked()
    {
        Application.Quit();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
