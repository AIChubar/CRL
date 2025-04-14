using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CustomVerticalLayoutGroup : MonoBehaviour
{
    public float verticalSpacing = 10f;
    public int symbolCount = 5;

    public void ApplyLayout()
    {
        RectTransform columnRect = GetComponent<RectTransform>();
        int childCount = columnRect.childCount;

        if (childCount == 0)
            return;

        float columnHeight = columnRect.rect.height;

        // Calculate total spacing height
        float totalSpacing = verticalSpacing * (childCount - 1);

        // Available height for symbols
        float availableHeight = columnHeight - totalSpacing;

        // Height and width of each symbol
        float symbolSize = availableHeight / childCount;

        // Starting Y offset from center
        float totalHeight = childCount * symbolSize + totalSpacing;
        float startY = totalHeight / 2f - symbolSize / 2f;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform symbol = columnRect.GetChild(i) as RectTransform;
            if (symbol == null || !symbol.gameObject.activeSelf)
                continue;

            // Set size (square)
            symbol.sizeDelta = new Vector2(symbolSize, symbolSize);

            // Centered anchoring
            symbol.anchorMin = symbol.anchorMax = new Vector2(0.5f, 0.5f);
            symbol.pivot = new Vector2(0.5f, 0.5f);

            // Position symbol vertically
            float y = startY - i * (symbolSize + verticalSpacing);
            symbol.anchoredPosition = new Vector2(0f, y);
        }
    }

    private void Start()
    {
        ApplyLayout();
    }
}