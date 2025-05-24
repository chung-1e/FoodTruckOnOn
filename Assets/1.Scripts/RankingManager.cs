using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class RankEntry
{
    public string nickname;
    public int score;
}

[System.Serializable]
public class RankData
{
    public List<RankEntry> ranks = new List<RankEntry>();
}

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;
    public List<RankEntry> rankList = new List<RankEntry>();
    private string savePath;

    [Header("��ŷ ����")]
    public int maxRankCount = 10;   // 10�� ������ ǥ��

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/rank.json";
            LoadRanks();
            Debug.Log("RankingManager Instance ���� �Ϸ�");
        }
        else
        {
            Debug.Log("RankingManager �ߺ� �߰�, �ı�");
            Destroy(gameObject);
            return;
        }
    }

    public void AddRank(string nickname, int score)
    {
        try
        {
            // ���� �г��� ó��                    // && : �׸��� , || : �Ǵ�
            if (string.IsNullOrEmpty(nickname) || string.IsNullOrWhiteSpace(nickname))
            {
                nickname = "Unknown";
            }

            // ���ο� �׸� ����
            RankEntry entry = new RankEntry
            {
                nickname = nickname.Trim(),
                score = score
            };

            // ��ŷ ����Ʈ �߰�
            rankList.Add(entry);

            // �������� �������� ����
            rankList = rankList.OrderByDescending(x => x.score).ToList();

            // ���� maxRankCount ������ŭ�� ����
            if (rankList.Count > maxRankCount)
            {
                rankList = rankList.Take(maxRankCount).ToList();
            }

            // ���Ͽ� ����
            SaveRanks();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"��ŷ �ε� ����: {e.Message}");
        }
    }

    public void SaveRanks()
    {
        try
        {
            RankData data = new RankData { ranks = rankList };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"��ŷ ���� �Ϸ�: {rankList.Count}�� �׸�");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"��ŷ ���� ����: {e.Message}");
        }
    }

    public void LoadRanks()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                if (!string.IsNullOrEmpty(json))
                {
                    RankData data = JsonUtility.FromJson<RankData>(json);
                    if (data != null && data.ranks != null)
                    {
                        rankList = data.ranks;
                        rankList = rankList.OrderByDescending(x => x.score).ToList();
                    }
                }
            }
            else
            {
                rankList = new List<RankEntry>();
            }
        }
        catch (System.Exception e)
        {
            rankList = new List<RankEntry>();
        }
    }

    public List<RankEntry> GetAllRanks()
    {
        return rankList;
    }

    [ContextMenu("Clear All Ranks")]
    public void ClearAllRanks()
    {
        rankList.Clear();
        SaveRanks();
    }
}
