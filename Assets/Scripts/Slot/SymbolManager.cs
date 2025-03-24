using System;
using System.Collections.Generic;
using UnityEngine;



public class SymbolManager
{
    private List<Symbol> symbols;

    public SymbolManager(List<Symbol> symbols)
    {
        this.symbols = symbols;
    }

    public Symbol GetRandomSymbol()
    {
        float totalProbability = 0f;
        foreach (var symbol in symbols)
        {
            totalProbability += symbol.probability;
        }

        float randomPoint = UnityEngine.Random.value * totalProbability;

        foreach (var symbol in symbols)
        {
            if (randomPoint < symbol.probability)
                return symbol;
            randomPoint -= symbol.probability;
        }
        return symbols[0];
    }
}