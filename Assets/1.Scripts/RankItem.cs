using TMPro;
using UnityEngine;

public class RankItem : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI scoreText;

    public void SetData(int rank, string nickname, int score)
{
Debug.Log($"[SetData] rank={rank}, nickname='{nickname}', score={score}");

    rankText.text = $"{rank}À§";
    nicknameText.text = nickname;
    scoreText.text = $"{score}Á¡";
}
}