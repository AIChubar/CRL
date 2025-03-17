using System;
using UnityEngine;

public class MonteCarloController : MonoBehaviour
{
    [Header("Monte Carlo Simulation Results")]
    public int simulationRuns = 1000;
    public float averageWin;
    public float winProbability;
    public float winCoef;
    
    public void RunMonteCarloSimulation()
    {
        int totalWin = 0;
        int totalWins = 0;
        
        for (int i = 0; i < simulationRuns; i++)
        {
            int result = GameManager.instance.slotMachine.SpinSlot(GameManager.instance.gameData.betAmount, true);
            totalWin += result;

            if (result > 0)
            {
                totalWins++;
            }
        }

        averageWin = (float)Math.Round((float)totalWin / simulationRuns, 2 );
        winProbability = (float)Math.Round((float)totalWins / simulationRuns * 100, 2);
        winCoef = (float)Math.Round(GameManager.instance.gameData.RTP * GameManager.instance.gameData.betAmount / averageWin, 2 );
        GameManager.instance.winCoef = winCoef;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
