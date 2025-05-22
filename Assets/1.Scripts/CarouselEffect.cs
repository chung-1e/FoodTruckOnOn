using UnityEngine;
using UnityEngine.UI;

public class CarouselEffect : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public float effectDistance = 300f; // 거리 기준
    public float centerSize = 600f; // 가운데 아이템 사이즈
    public float sideSize = 480f;   // 옆 아이템 사이즈

    void Update()
    {
        float viewportCenterX = scrollRect.viewport.rect.width / 2f;

        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform item = content.GetChild(i) as RectTransform;
            Vector3 itemLocalPos = content.InverseTransformPoint(item.position);
            float distance = Mathf.Abs(itemLocalPos.x - viewportCenterX);

            // distance에 따라 크기 보간 (0~effectDistance)
            float t = Mathf.Clamp01(distance / effectDistance);
            float size = Mathf.Lerp(centerSize, sideSize, t);

            // 크기 변경
            item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }
    }
}