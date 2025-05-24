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

    [Header("랭킹 설정")]
    public int maxRankCount = 10;   // 10등 까지만 표시

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/rank.json";
            LoadRanks();
            Debug.Log("RankingManager Instance 생성 완료");
        }
        else
        {
            Debug.Log("RankingManager 중복 발견, 파괴");
            Destroy(gameObject);
            return;
        }
    }

    public void AddRank(string nickname, int score)
    {
        try
        {
            // 공백 닉네임 처리                    // && : 그리고 , || : 또는
            if (string.IsNullOrEmpty(nickname) || string.IsNullOrWhiteSpace(nickname))
            {
                nickname = "Unknown";
            }

            // 새로운 항목 생성
            RankEntry entry = new RankEntry
            {
                nickname = nickname.Trim(),
                score = score
            };

            // 랭킹 리스트 추가
            rankList.Add(entry);

            // 점수별로 내림차순 정렬
            rankList = rankList.OrderByDescending(x => x.score).ToList();

            // 상위 maxRankCount 갯수만큼만 유지
            if (rankList.Count > maxRankCount)
            {
                rankList = rankList.Take(maxRankCount).ToList();
            }

            // 파일에 저장
            SaveRanks();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"랭킹 로드 실패: {e.Message}");
        }
    }

    public void SaveRanks()
    {
        try
        {
            RankData data = new RankData { ranks = rankList };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"랭킹 저장 완료: {rankList.Count}개 항목");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"랭킹 저장 실패: {e.Message}");
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
