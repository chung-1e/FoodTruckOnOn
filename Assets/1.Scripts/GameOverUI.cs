using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SetScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"최종 점수: {score}";
        }
    }
public static class GameData
{
    public static int finalScore = 0;
}

}