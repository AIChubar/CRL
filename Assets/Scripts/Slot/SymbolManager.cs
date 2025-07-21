using System;
using System.Collections.Generic;
using UnityEngine;



public class SymbolManager
{
    private List<Symbol> symbols;
    private Dictionary<Symbol, float> symbolProbabilities; // Precomputed probabilities
    private List<KeyValuePair<Symbol, float>> cumulativeList; // Precomputed cumulative probabilities
    private GameData gameData;
    
    public SymbolManager(List<Symbol> symbols, GameData gameData)
    {
        this.symbols = symbols;
        this.gameData = gameData;
        ComputeSymbolProbabilities();
    }

    public Symbol GetRandomSymbol()
    {
        float rand = GameManager.instance.rngManager.NextFloat();
        foreach (var kvp in cumulativeList)
        {
            if (rand <= kvp.Value)
            {
                return kvp.Key;
            }
        }
        return null;
    }

    public Symbol GetRandomSymbolUnweighted(Symbol symbolToExclude = null, bool affectedByLuck = false)
    {
        List<Symbol> possibleSymbols = new List<Symbol>(symbols);
        if (symbolToExclude != null)
            possibleSymbols.Remove(symbolToExclude);

        if (!affectedByLuck)
        {
            int rand = GameManager.instance.rngManager.NextInt(0, possibleSymbols.Count);
            return possibleSymbols[rand];
        }

        float luck = gameData.wildLuck.GetValue(includeTemporary: true);
        luck = Mathf.Max(0f, luck); // clamp to non-negative

        if (luck > 10000f)
        {
            // Try get wild symbols only
            List<Symbol> wildSymbols = possibleSymbols.FindAll(s => s.isWild);
            if (wildSymbols.Count > 0)
            {
                int wildIndex = GameManager.instance.rngManager.NextInt(0, wildSymbols.Count);
                return wildSymbols[wildIndex];
            }
            // fallback to normal random if no wild symbols
            int randFallback = GameManager.instance.rngManager.NextInt(0, possibleSymbols.Count);
            return possibleSymbols[randFallback];
        }
        
        // Step 1: Assign weights to each symbol
        List<float> weights = new List<float>(possibleSymbols.Count);
        float totalWeight = 0f;

        foreach (var sym in possibleSymbols)
        {
            float weight = sym.isWild ? luck : 1f;
            weights.Add(weight);
            totalWeight += weight;
        }

        // Step 2: Normalize weights and do weighted selection
        float roll = GameManager.instance.rngManager.NextFloat(0f, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < possibleSymbols.Count; i++)
        {
            cumulative += weights[i];
            if (roll < cumulative)
                return possibleSymbols[i];
        }

        // Should not happen but fallback
        return possibleSymbols[possibleSymbols.Count - 1];
    }


    
    public void ComputeSymbolProbabilities()
    {
        symbolProbabilities = new Dictionary<Symbol, float>();
        cumulativeList = new List<KeyValuePair<Symbol, float>>();

        float assignedProb = 0f;
        List<Symbol> unassignedSymbols = new List<Symbol>();

        // Identify assigned probabilities and collect unassigned symbols
        foreach (var symbol in symbols)
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
    
    internal Symbol GetSymbolByIndex(int i)
    {
        return symbols[i];
    }
}