using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Ÿ�̸� ����")]
    public Slider timerSlider;          // Ÿ�̸� �����̴�
    public float gameTime = 120f;       // ���� �ð� (2�� = 120��)
    private float timeRemaining;        // ���� �ð�
    private bool isGameActive = false;  // ���� Ȱ��ȭ ����

    [Header("���ھ� ����")]
    public Text scoreText;   // ���ھ� �ؽ�Ʈ
    private int currentScore = 0;       // ���� ���ھ�

    [Header("UI �г�")]
    public GameObject gameOverPanel;    // ���� ���� UI

    [Header("���� �ý���")]
    public RecipeManager recipeManager; // ������ �Ŵ���
    public FeverSystem feverSystem;     // �ǹ� �ý���
    public RollDice diceRoller;         // �ֻ��� �ý���

    [Header("����/�г�Ƽ ����")]
    // �ֻ��� ���� ���� ���� �� �ð� ���� (�ε��� 0 = �� 1)
    private int[] timeRewardsByDice = { 3, 4, 6, 8, 10, 12 };
    // �ֻ��� ���� ���� ���� �� �ð� �г�Ƽ
    private int[] timePenaltiesByDice = { 2, 3, 4, 5, 6, 7 };
    // �ֻ��� ���� ���� ���� �� ���� ����
    private int[] scoreRewardsByDice = { 10, 20, 30, 40, 50, 60 };
    // �ֻ��� ���� ���� ���� �� ���� �г�Ƽ
    private int[] scorePenaltiesByDice = { 5, 10, 15, 20, 25, 30 };

   [Header("�г��� ����")]
    public TMP_InputField nicknameInputField; 

    void Start()
    {
        // ������Ʈ Ȯ��
        if (timerSlider == null)
            Debug.LogError("Ÿ�̸� �����̴��� �Ҵ���� �ʾҽ��ϴ�!");
        if (scoreText == null)
            Debug.LogError("���ھ� �ؽ�Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");

        // ���� ���� ����
        InitializeGame();

    
    }

    void Update()
    {
        if (isGameActive)
        {
            UpdateTimer();
        }
     
    
    }

    // ���� �ʱ�ȭ
    public void InitializeGame()
    {
        // Ÿ�̸� �ʱ�ȭ
        timeRemaining = gameTime;
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;

        // ���ھ� �ʱ�ȭ
        currentScore = 0;
        UpdateScoreDisplay();

        // UI �г� ����
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // ���� ����
        isGameActive = true;

        // �ʿ�� ������ �ý��۵� �ʱ�ȭ
        if (recipeManager != null)
            Debug.Log("������ �Ŵ��� �غ� �Ϸ�");
        if (feverSystem != null)
            Debug.Log("�ǹ� �ý��� �غ� �Ϸ�");
        if (diceRoller != null)
            Debug.Log("�ֻ��� �ý��� �غ� �Ϸ�");
    }

    // Ÿ�̸� ������Ʈ
    void UpdateTimer()
    {
        // �ǹ� Ÿ�� �߿� Ÿ�̸� ����
        if (feverSystem != null && feverSystem.IsInFever())
        {
            return; // �ǹ� Ÿ�� �߿� �ð��� �帣�� �ʰ�
        }

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerSlider.value = timeRemaining;
        }
        else
        {
            timeRemaining = 0;
            timerSlider.value = 0;
            EndGame();
        }
    }

    // �ð� ���� (����/���п� ���� ����/�г�Ƽ)
    public void ChangeTime(int diceValue, bool isSuccess)
    {
        // �ǹ� Ÿ�� �߿� Ÿ�̸� ����
        if (feverSystem != null && feverSystem.IsInFever())
        {
            return; // �ǹ� Ÿ�� �߿� �ð��� �帣�� �ʰ�
        }

        // �迭 �ε����� 0���� �����ϹǷ� ����
        int index = Mathf.Clamp(diceValue - 1, 0, 5);

        if (isSuccess)
        {
            // ���� �� - �ð� �߰�
            int timeReward = timeRewardsByDice[index];
            timeRemaining += timeReward;

            Debug.Log("����! �ð� ����: +" + timeReward + "�� (���� �ð�: " + timeRemaining + ")");
        }
        else
        {
            // ���� �� - �ð� ��� ���� (�г�Ƽ)
            int timePenalty = timePenaltiesByDice[index];

            // ���� �� �� ����
            float beforeChange = timeRemaining;

            // �ð� ���� ����
            timeRemaining -= timePenalty;
            timeRemaining = Mathf.Max(0, timeRemaining); // ���� ����

            Debug.Log("����! �ð� �г�Ƽ: -" + timePenalty + "�� (" + beforeChange + " �� " + timeRemaining + ")");
        }

        // �����̴� ���� ������Ʈ
        if (timerSlider != null)
        {
            // �����̴� ���� ��� ������Ʈ
            timerSlider.value = timeRemaining;

            // UI ��� ������ ���� �߰� �ڵ�
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)timerSlider.transform);

            Debug.Log("Ÿ�̸� �����̴� �� ����: " + timerSlider.value);
        }
        else
        {
            Debug.LogError("Ÿ�̸� �����̴��� null�Դϴ�!");
        }

        // Time.timeScale�� 0���� Ȯ�� (�Ͻ����� ��������)
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            Debug.LogWarning("����: Time.timeScale�� 0�Դϴ�. Ÿ�̸� ������Ʈ�� ����� �۵����� ���� �� �ֽ��ϴ�!");
        }
    }

    // ���ھ� ������Ʈ (����/���п� ���� ����/�г�Ƽ)
    public void UpdateScore(int diceValue, bool isSuccess)
    {
        // �迭 �ε����� 0���� �����ϹǷ� ����
        int index = Mathf.Clamp(diceValue - 1, 0, 5);

        int scoreChange = 0;
        if (isSuccess)
        {
            scoreChange = scoreRewardsByDice[index];
        }
        else
        {
            scoreChange = -scorePenaltiesByDice[index];
        }

        // ���� ������Ʈ (�ּ� 0)
        currentScore = Mathf.Max(0, currentScore + scoreChange);
        UpdateScoreDisplay();

        // ���� ���� �α�
        Debug.Log((isSuccess ? "����" : "����") +
                 $" - �ֻ��� {diceValue}: ���� {(scoreChange >= 0 ? "+" : "")}{scoreChange}" +
                 $" (���� ����: {currentScore})");
    }

    // ���ھ� ǥ�� ������Ʈ
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    // �ܹ��� �ϼ�/���� ó��
    public void OnBurgerResult(int diceValue, bool isSuccess)
    {
        // ���� ������Ʈ
        UpdateScore(diceValue, isSuccess);

        // �ð� ����
        ChangeTime(diceValue, isSuccess);
    }

    // ���� ����
    void EndGame()
    {
        if (!isGameActive)
            return;

        isGameActive = false;
       
         // ���� ������ ScoreManager�� ����
    ScoreManager.Instance.currentScore = currentScore;
    // ��ŷ �г��� ����
    string nickname = PlayerPrefs.GetString("PlayerNickname", "Unknown");
    
    RankingManager.Instance.AddRank(nickname, currentScore);

        Debug.Log("���� ����! ���� ���ھ�: " + currentScore);

        // ���� ���� �г� Ȱ��ȭ
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
           
        }

        // ���� �Ͻ����� (���� ����)
        Time.timeScale = 1;

     
    }

    // ����� ��ư (���� ���� �гο��� ���)
    public void RestartGame()
    {
        // ���� �ð� ����
        Time.timeScale = 1;

        // ���� �ٽ� �ʱ�ȭ
        InitializeGame();
    }
  

public void SavePlayerNickname()
{
    string playerNickname = nicknameInputField.text;
    if (!string.IsNullOrEmpty(playerNickname))
    {
        PlayerPrefs.SetString("PlayerNickname", playerNickname);
        PlayerPrefs.Save();
        Debug.Log("�г��� ���� �Ϸ�: " + playerNickname);
    }
    else
    {
        Debug.LogWarning("�г����� ����ֽ��ϴ�.");
    }
}


}


