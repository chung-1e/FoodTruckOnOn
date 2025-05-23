using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

   private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("RankingManager Instance ���� �Ϸ�");
    }
    else
    {
        Debug.Log("RankingManager �ߺ� �߰�, �ı�");
        Destroy(gameObject);
    }

    savePath = Application.persistentDataPath + "/rank.json";
    LoadRanks();
}
    public void AddRank(string nickname, int score)
    {
        rankList.Add(new RankEntry { nickname = nickname, score = score });
        rankList.Sort((a, b) => b.score.CompareTo(a.score)); // ���� ���� ������ ����
        if (rankList.Count > 10)
            rankList.RemoveAt(10); // �ִ� 10���� ����

        SaveRanks();
    }

    public void SaveRanks()
    {
        string json = JsonUtility.ToJson(new RankData { ranks = rankList }, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadRanks()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            rankList = JsonUtility.FromJson<RankData>(json).ranks;
        }
    }


}
