using System.Collections.Generic;
using UnityEngine;

public static class SymbolChanger
{
  
    public static string GetNewSymbol(string currentSymbol, string[] slotSymbols, string wildSymbol)
    {
        List<string> possibleSymbols = new List<string>(slotSymbols) { wildSymbol };
        possibleSymbols.Remove(currentSymbol);
        return possibleSymbols[Random.Range(0, possibleSymbols.Count)];
    }
}