using UnityEngine;
using UnityEngine.UI;

public class CarouselController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;

    private int currentIndex = 0;
    private int itemCount;

    void Start()
    {
        itemCount = content.childCount;
        MoveToIndex(currentIndex);
    }

    public void MoveToIndex(int index)
    {
        if (index < 0) index = 0;
        if (index >= itemCount) index = itemCount - 1;

        currentIndex = index;

        RectTransform target = content.GetChild(currentIndex) as RectTransform;

        float contentWidth = content.rect.width;
        float viewportWidth = scrollRect.viewport.rect.width;
        Vector3 targetLocalPos = target.localPosition;

        float targetNormalizedPos = (targetLocalPos.x * -1f - viewportWidth / 2f) / (contentWidth - viewportWidth);
        targetNormalizedPos = Mathf.Clamp01(targetNormalizedPos);

        scrollRect.horizontalNormalizedPosition = targetNormalizedPos;
    }

    public void OnLeftButtonClicked()
    {
        MoveToIndex(currentIndex - 1);
    }

    public void OnRightButtonClicked()
    {
        MoveToIndex(currentIndex + 1);
    }
}
