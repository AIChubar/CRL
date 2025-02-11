using System.Collections.Generic;
using UnityEngine;

public static class SymbolChanger
{
    /// <summary>
    /// Выбирает новый символ для смены, исключая текущий.
    /// </summary>
    /// <param name="currentSymbol">Текущий символ.</param>
    /// <param name="slotSymbols">Массив обычных символов.</param>
    /// <param name="wildSymbol">Wild-символ.</param>
    /// <returns>Новый выбранный символ.</returns>
    public static string GetNewSymbol(string currentSymbol, string[] slotSymbols, string wildSymbol)
    {
        // Создаем список возможных символов: обычные + wild
        List<string> possibleSymbols = new List<string>(slotSymbols) { wildSymbol };
        // Удаляем текущий символ, чтобы гарантировать изменение
        possibleSymbols.Remove(currentSymbol);
        // Выбираем случайный символ из оставшихся
        return possibleSymbols[Random.Range(0, possibleSymbols.Count)];
    }
}