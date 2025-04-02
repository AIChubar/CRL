using System.Collections.Generic;
using UnityEngine;

public class SlotReel
{

    public List<Symbol> reelSymbols; // List of possible symbols
    private Dictionary<Symbol, float> symbolProbabilities; // Precomputed probabilities
    private List<KeyValuePair<Symbol, float>> cumulativeList; // Precomputed cumulative probabilities

    public SlotReel(List<Symbol> reelSymbols)
    {
        this.reelSymbols = reelSymbols;
        ComputeSymbolProbabilities();
    }

    
    public void ComputeSymbolProbabilities()
    {
        symbolProbabilities = new Dictionary<Symbol, float>();
        cumulativeList = new List<KeyValuePair<Symbol, float>>();

        float assignedProb = 0f;
        List<Symbol> unassignedSymbols = new List<Symbol>();

        // Identify assigned probabilities and collect unassigned symbols
        foreach (var symbol in reelSymbols)
        {
            if (symbol.probability > 0)
            {
                symbolProbabilities[symbol] = symbol.probability;
                assignedProb += symbol.probability;
            }
            else
            {
                unassignedSymbols.Add(symbol);
            }
        }

        // Distribute remaining probability among unassigned symbols
        float remainingProb = Mathf.Max(0, 1f - assignedProb);
        float equalProb = unassignedSymbols.Count > 0 ? remainingProb / unassignedSymbols.Count : 0;

        foreach (var symbol in unassignedSymbols)
        {
            symbolProbabilities[symbol] = equalProb;
        }

        // Build cumulative probability list for fast weighted selection
        float cumulativeSum = 0f;
        foreach (var kvp in symbolProbabilities)
        {
            cumulativeSum += kvp.Value;
            cumulativeList.Add(new KeyValuePair<Symbol, float>(kvp.Key, cumulativeSum));
        }
    }

    public List<Symbol> GenerateSymbols(int numberOfSymbols)
    {
        List<Symbol> symbols = new List<Symbol>();

        for (int i = 0; i < numberOfSymbols; i++)
        {
            float rand = Random.value;
            foreach (var kvp in cumulativeList)
            {
                if (rand <= kvp.Value)
                {
                    symbols.Add(kvp.Key);
                    break;
                }
            }
        }

        return symbols;
    }
}