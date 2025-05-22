using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingPopup : MonoBehaviour
{
    public Transform contentParent; // 랭킹 항목들을 넣을 부모 오브젝트
    public GameObject rankItemPrefab; // 랭킹 항목 하나의 프리팹

    void OnEnable()
    {
        UpdateRankUI();
    }

    public void UpdateRankUI()
    {
        // 기존 항목 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 항목 생성
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
