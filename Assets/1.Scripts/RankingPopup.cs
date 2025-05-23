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
        RankItem item = go.GetComponent<RankItem>();
        item.SetData(i + 1, ranks[i].nickname, ranks[i].score);
    }
}

}
