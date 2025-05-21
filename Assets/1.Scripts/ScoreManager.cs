using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int currentScore = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� �ٲ� �ı����� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}