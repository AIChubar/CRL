using UnityEngine;

public class ManualVerticalLayout : MonoBehaviour
{
    public float spacing = 5f;
    public float topPadding = 10f;
    public float bottomPadding = 10f;

    public void ApplyLayout()
    {
        RectTransform parent = GetComponent<RectTransform>();
        float currentY = -topPadding;

        for (int i = 0; i < parent.childCount; i++)
        {
            RectTransform child = parent.GetChild(i) as RectTransform;
            if (child == null || !child.gameObject.activeSelf) continue;

            float height = child.rect.height;
            child.anchorMin = new Vector2(0.5f, 1);
            child.anchorMax = new Vector2(0.5f, 1);
            child.pivot = new Vector2(0.5f, 1);

            child.anchoredPosition = new Vector2(0, currentY);
            currentY -= height + spacing;
        }

        float totalHeight = Mathf.Abs(currentY) + bottomPadding;
        parent.sizeDelta = new Vector2(parent.sizeDelta.x, totalHeight);
    }
}
