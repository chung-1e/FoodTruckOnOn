using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
   {
    if (scoreText == null)
    {
        Debug.LogError("scoreText�� ������� �ʾҽ��ϴ�! Inspector���� �������ּ���.");
        return;
    }

    if (ScoreManager.Instance == null)
    {
        Debug.LogError("ScoreManager�� null�Դϴ�! �� ���� ��� �ִ��� Ȯ�����ּ���.");
        return;
    }

    scoreText.text = "���� ����: " + ScoreManager.Instance.currentScore;
}


}
