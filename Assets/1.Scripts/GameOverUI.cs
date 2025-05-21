using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
   {
    if (scoreText == null)
    {
        Debug.LogError("scoreText가 연결되지 않았습니다! Inspector에서 연결해주세요.");
        return;
    }

    if (ScoreManager.Instance == null)
    {
        Debug.LogError("ScoreManager가 null입니다! 이 씬에 살아 있는지 확인해주세요.");
        return;
    }

    scoreText.text = "최종 점수: " + ScoreManager.Instance.currentScore;
}


}
