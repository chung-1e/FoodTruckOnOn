using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingPopup : MonoBehaviour
{
    public Transform contentParent; 
    public GameObject rankItemPrefab; 

    void OnEnable()
    {
        UpdateRankUI();
    }

    public void UpdateRankUI() 
    {
        // RankingManager가 null인지 확인
        if (RankingManager.Instance == null)
        {
            Debug.LogError("RankingManager.Instance가 null입니다!");
            return;
        }

        // 기존 항목 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 항목 생성
        List<RankEntry> ranks = RankingManager.Instance.GetAllRanks();
        for (int i = 0; i < ranks.Count; i++)
        {
            GameObject go = Instantiate(rankItemPrefab, contentParent);
            RankItem item = go.GetComponent<RankItem>();
            if (item != null)
            {
                item.SetData(i + 1, ranks[i].nickname, ranks[i].score);
            }
            else
            {
                Debug.LogError("RankItem 컴포넌트를 찾을 수 없습니다!");
            }
        }
    }
}
