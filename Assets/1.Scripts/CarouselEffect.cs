using UnityEngine;
using UnityEngine.UI;

public class CarouselEffect : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public float effectDistance = 300f; // �Ÿ� ����
    public float centerSize = 600f; // ��� ������ ������
    public float sideSize = 480f;   // �� ������ ������

    void Update()
    {
        float viewportCenterX = scrollRect.viewport.rect.width / 2f;

        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform item = content.GetChild(i) as RectTransform;
            Vector3 itemLocalPos = content.InverseTransformPoint(item.position);
            float distance = Mathf.Abs(itemLocalPos.x - viewportCenterX);

            // distance�� ���� ũ�� ���� (0~effectDistance)
            float t = Mathf.Clamp01(distance / effectDistance);
            float size = Mathf.Lerp(centerSize, sideSize, t);

            // ũ�� ����
            item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }
    }
}