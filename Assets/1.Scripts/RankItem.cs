using TMPro;
using UnityEngine;

public class RankItem : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI playTimeText;
    public TextMeshProUGUI scoreText;

    public void SetData(int rank, string nickname,float playTime, int score)
    {
        Debug.Log($"[SetData] rank={rank}, nickname='{nickname}',playTime={playTime:F1}s, score={score}");

        rankText.text = $"{rank}À§";
        nicknameText.text = nickname;
        playTimeText.text = FormatTime(playTime);
        scoreText.text = $"{score}Á¡";
    }
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}