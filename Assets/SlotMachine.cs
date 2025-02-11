using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotMachine : MonoBehaviour
{
    // Объект конфигурации, созданный как ScriptableObject (SlotConfig)
    public SlotConfig firstSlotConfig;
    public SlotConfig secondSlotConfig;

    
    private SlotConfig currentSlotConfig;
    public GameObject slotPanel;            // UI-панель с Grid Layout Group
    public GameObject slotSymbolPrefab;     // Префаб для отображения символов (с компонентом TMP_Text и Button)

    private string[,] slotGrid;             // Матрица для хранения символов слота

    private List<SlotReel> reels;
    
    private void Start()
    {
        currentSlotConfig = firstSlotConfig;
        reels = new List<SlotReel>();
        for (int i = 0; i < currentSlotConfig.columns; i++)
        {
            reels.Add(new SlotReel(currentSlotConfig.rows, currentSlotConfig.wildChance, currentSlotConfig.wildSymbol, currentSlotConfig.slotSymbols));
        }
        // Инициализируем матрицу согласно размерам, указанным в slotConfig
        slotGrid = new string[currentSlotConfig.rows, currentSlotConfig.columns];
    }

    // Функция SpinSlot запускает вращение слота и возвращает сумму выигрыша
    public int SpinSlot(int betAmount, bool simulateOnly = false)
    {
        if (!simulateOnly)
        {
            foreach (Transform child in slotPanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
        for (int col = 0; col < currentSlotConfig.columns; col++)
        {
            string[] reelSymbols = reels[col].GenerateSymbols();
            for (int row = 0; row < currentSlotConfig.rows; row++)
            {
                slotGrid[row, col] = reelSymbols[row];
            }
        }

        if (!simulateOnly)
        {
            for (int row = 0; row < currentSlotConfig.rows; row++)
            {
                for (int col = 0; col < currentSlotConfig.columns; col++)
                {
                    GameObject newSymbol = Instantiate(slotSymbolPrefab, slotPanel.transform);
                    newSymbol.GetComponent<TMP_Text>().text = slotGrid[row, col];
                    int r = row, c = col;
                    newSymbol.GetComponent<Button>().onClick.AddListener(() => ChangeSymbol(r, c));
                }
            }
        }

        // Расчет выигрыша с использованием SlotCalculator
        return SlotCalculator.CalculateWin(slotGrid, betAmount, currentSlotConfig.rows, currentSlotConfig.columns, currentSlotConfig.wildSymbol, currentSlotConfig.slotSymbols);
    }


    
    public int CalculateWin(int betAmount)
    {
        // Передаем в метод: матрица слота, ставка, количество строк, количество столбцов, wild-символ и обычные символы.
        return SlotCalculator.CalculateWin(slotGrid, betAmount, currentSlotConfig.rows, currentSlotConfig.columns, currentSlotConfig.wildSymbol, currentSlotConfig.slotSymbols);
        
    }

    // Функция смены символа в указанной позиции (вызывается при клике по символу, если включен режим смены)
    public void ChangeSymbol(int row, int col)
    {
        if (!GameManager.instance.isChangingSymbol) return;
        GameManager.instance.isChangingSymbol = false;

        // Создаем список возможных символов (с wild) из конфигурации и удаляем текущий символ
        List<string> possibleSymbols = new List<string>(currentSlotConfig.slotSymbols) { currentSlotConfig.wildSymbol };
        possibleSymbols.Remove(slotGrid[row, col]);
        slotGrid[row, col] = possibleSymbols[Random.Range(0, possibleSymbols.Count)];

        // Обновляем UI для конкретного символа: рассчитываем индекс как row * columns + col
        Transform symbolTransform = slotPanel.transform.GetChild(row * currentSlotConfig.columns + col);
        symbolTransform.GetComponent<TMP_Text>().text = slotGrid[row, col];
        GameManager.instance.FinishChangingSymbol();
    }
}
