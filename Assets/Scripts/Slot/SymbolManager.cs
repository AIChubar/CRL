using System;
using System.Collections.Generic;
using UnityEngine;



public class SymbolManager
{
    private List<Symbol> symbols;
    private Dictionary<Symbol, float> symbolProbabilities; // Precomputed probabilities
    private List<KeyValuePair<Symbol, float>> cumulativeList; // Precomputed cumulative probabilities

    public SymbolManager(List<Symbol> symbols)
    {
        this.symbols = symbols;
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

    public Symbol GetRandomSymbolUnweighted(Symbol symbolToExclude = null)
    {
        List<Symbol> possibleSymbols = new List<Symbol>(symbols);
        possibleSymbols.Remove(symbolToExclude);
        int rand = GameManager.instance.rngManager.NextInt(0, possibleSymbols.Count);
        return possibleSymbols[rand];
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