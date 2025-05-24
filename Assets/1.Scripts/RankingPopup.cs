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
        // RankingManager�� null���� Ȯ��
        if (RankingManager.Instance == null)
        {
            Debug.LogError("RankingManager.Instance�� null�Դϴ�!");
            return;
        }

        // ���� �׸� ����
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ���ο� �׸� ����
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
                Debug.LogError("RankItem ������Ʈ�� ã�� �� �����ϴ�!");
            }
        }
    }
}
