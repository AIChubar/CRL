using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CustomHorizontalLayoutGroup : MonoBehaviour
{
    public float spacing = 10f;
    public float paddingTop = 0f;
    public float paddingBottom = 0f;
    public TextAnchor childAlignment = TextAnchor.MiddleCenter;

    public void ApplyLayout()
    {
        RectTransform parentRect = GetComponent<RectTransform>();
        int childCount = parentRect.childCount;

        if (childCount == 0)
            return;

        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        float totalSpacing = spacing * (childCount - 1);
        float columnWidth = (parentWidth - totalSpacing) / childCount;
        float columnHeight = parentHeight - paddingTop - paddingBottom;

        // Start positioning from the center and distribute columns evenly
        float totalWidth = childCount * columnWidth + totalSpacing;
        float startX = -totalWidth / 2f + columnWidth / 2f;

        float yOffset = GetVerticalOffset(columnHeight);

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = parentRect.GetChild(i) as RectTransform;
            if (child == null || !child.gameObject.activeSelf)
                continue;

            // Set size
            child.sizeDelta = new Vector2(columnWidth, columnHeight);

            // Set position relative to center
            child.anchoredPosition = new Vector2(startX + i * (columnWidth + spacing), yOffset);
        }
    }

    private float GetVerticalOffset(float columnHeight)
    {
        switch (childAlignment)
        {
            case TextAnchor.UpperCenter:
            case TextAnchor.UpperLeft:
            case TextAnchor.UpperRight:
                return -paddingTop - columnHeight / 2f;
            case TextAnchor.MiddleCenter:
            case TextAnchor.MiddleLeft:
            case TextAnchor.MiddleRight:
                return (paddingBottom - paddingTop) / 2f;
            case TextAnchor.LowerCenter:
            case TextAnchor.LowerLeft:
            case TextAnchor.LowerRight:
                return paddingBottom + columnHeight / 2f;
            default:
                return 0f;
        }
    }

    private void Start()
    {
        ApplyLayout();
    }
}
