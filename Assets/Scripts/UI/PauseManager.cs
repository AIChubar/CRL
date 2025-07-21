using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public Button restartButton;
    public Button nextLevelButton;

    public GameObject winLoseMenu;
    public GameObject pauseMenu;

    [SerializeField] private StatsMenu statsMenu;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
        winLoseMenu.SetActive(false);
    }

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
        if (statsMenu.IsOpen)
        {
            statsMenu.Hide();
        }
        else
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }
    }
}