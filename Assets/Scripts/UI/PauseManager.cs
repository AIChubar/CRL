using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public Button restartButton;
    public Button nextLevelButton;
    
    public GameObject winLoseMenu;
    public GameObject statsMenu;
    public GameObject pauseMenu;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
        winLoseMenu.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerInput.Player.Pause.triggered)
        {
            PauseGame();
        }
    }
    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
    public void PauseGame()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
}
