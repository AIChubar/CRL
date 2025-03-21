using UnityEngine;

public class SlotReel
{
    public int numberOfSymbols;     // Например, 3 символа (количество строк)
    public float wildChance;        // Шанс выпадения Wild (например, 0.02)
    public string wildSymbol;       // Wild-символ, например "⭐"
    public string[] reelSymbols;    // Массив обычных символов (например, {"🍎", "🍒", "🍋", "🍉", "🍌"})

    public SlotReel(int numberOfSymbols, float wildChance, string wildSymbol, string[] reelSymbols)
    {
        this.numberOfSymbols = numberOfSymbols;
        this.wildChance = wildChance;
        this.wildSymbol = wildSymbol;
        this.reelSymbols = reelSymbols;
    }

    /// <summary>
    /// Генерирует набор символов для данного барабана.
    /// </summary>
    public string[] GenerateSymbols()
    {
        string[] symbols = new string[numberOfSymbols];
        for (int i = 0; i < numberOfSymbols; i++)
        {
            if (Random.value < wildChance)
            {
                symbols[i] = wildSymbol;
            }
            else
            {
                int randomIndex = Random.Range(0, reelSymbols.Length);
                symbols[i] = reelSymbols[randomIndex];
            }
        }
        return symbols;
    }
}