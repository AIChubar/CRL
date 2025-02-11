using System.Collections.Generic;

public static class SlotCalculator
{
    /// <summary>
    /// Calculates the win amount based on the slot grid.
    /// </summary>
    /// <param name="slotGrid">2D array of symbols representing the slot grid.</param>
    /// <param name="betAmount">Bet amount per spin.</param>
    /// <param name="rows">Number of rows in the grid.</param>
    /// <param name="columns">Number of columns in the grid.</param>
    /// <param name="wildSymbol">The wild symbol that can substitute any symbol.</param>
    /// <param name="slotSymbols">Array of regular slot symbols.</param>
    /// <returns>Total win amount.</returns>
    public static int CalculateWin(string[,] slotGrid, int betAmount, int rows, int columns, string wildSymbol, string[] slotSymbols)
    {
        int totalWin = 0;
        // Создаем словарь для подсчета количества символов в каждом столбце
        Dictionary<string, int[]> symbolCountPerColumn = new Dictionary<string, int[]>();

        // Инициализируем счетчики для обычных символов
        foreach (string symbol in slotSymbols)
        {
            symbolCountPerColumn[symbol] = new int[columns];
        }
        // Инициализируем счетчик для wild-символа (если его еще нет)
        if (!symbolCountPerColumn.ContainsKey(wildSymbol))
            symbolCountPerColumn[wildSymbol] = new int[columns];

        // Подсчитываем в каждом столбце количество каждого символа
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                string symbol = slotGrid[row, col];
                if (symbolCountPerColumn.ContainsKey(symbol))
                    symbolCountPerColumn[symbol][col]++;
                // Если встретился новый символ, можно добавить обработку, но для простоты предполагаем, что это не происходит
            }
        }

        // Для каждого символа (кроме wild) добавляем в каждый столбец количество wild-символов
        foreach (var entry in symbolCountPerColumn)
        {
            string symbol = entry.Key;
            int[] counts = entry.Value;
            if (symbol != wildSymbol)
            {
                // Добавляем в каждый столбец количество wild-символов
                for (int col = 0; col < columns; col++)
                {
                    counts[col] += symbolCountPerColumn[wildSymbol][col];
                }
            }
            // Если символ (с учетом wild) присутствует во всех столбцах, считаем множитель и добавляем выигрыш
            bool valid = true;
            int multiplier = 1;
            for (int col = 0; col < columns; col++)
            {
                if (counts[col] <= 0)
                {
                    valid = false;
                    break;
                }
                multiplier *= counts[col];
            }
            if (valid)
            {
                totalWin += betAmount * multiplier;
            }
        }

        return totalWin;
    }
}
