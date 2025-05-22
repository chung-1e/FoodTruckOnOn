using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingPopup : MonoBehaviour
{
    public Transform contentParent; // ��ŷ �׸���� ���� �θ� ������Ʈ
    public GameObject rankItemPrefab; // ��ŷ �׸� �ϳ��� ������

    void OnEnable()
    {
        UpdateRankUI();
    }

    public void UpdateRankUI()
    {
        // ���� �׸� ����
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ���ο� �׸� ����
        List<RankEntry> ranks = RankingManager.Instance.rankList;
        for (int i = 0; i < ranks.Count; i++)
        {
            GameObject go = Instantiate(rankItemPrefab, contentParent);
            TextMeshProUGUI[] texts = go.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 2)
            {
                texts[0].text = $"{i + 1}. {ranks[i].nickname}";
                texts[1].text = ranks[i].score.ToString();
            }
        }
    }
}
