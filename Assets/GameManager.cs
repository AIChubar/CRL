using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [HideInInspector]public static GameManager instance;
  
    [SerializeField]public SlotUIManager slotUIManager;
    
    public GameData gameData;
    
    public SlotMachine slotMachine;

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
        slotUIManager.Setup(gameData, slotMachine);
    }
}