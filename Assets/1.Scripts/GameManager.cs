using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Ÿ�̸� ����")]
    public Slider timerSlider;          // Ÿ�̸� �����̴�
    public Text timerText;   // Ÿ�̸� �ؽ�Ʈ (�߰�)
    public float gameTime = 120f;       // ���� �ð� (2�� = 120��)
    private float timeRemaining;        // ���� �ð�
    private bool isGameActive = false;  // ���� Ȱ��ȭ ����

    [Header("���ھ� ����")]
    public Text scoreText;   // ���ھ� �ؽ�Ʈ
    public Text finalScoreText; // ���� ���ھ� �ؽ�Ʈ (���� ���� ��)
    private int currentScore = 0;       // ���� ���ھ�

    [Header("UI �г�")]
    public GameObject gamePanel;        // ���� �� UI
    public GameObject gameOverPanel;    // ���� ���� UI
    public Text timeChangeText; // �ð� ���� �˸� �ؽ�Ʈ (�߰�)

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
    
    void Start()
    {
        // ������Ʈ Ȯ��
        if (timerSlider == null)
            Debug.LogError("Ÿ�̸� �����̴� �Ҵ���� �ʽ��ϴ�!");
        if (scoreText == null)
            Debug.LogError("���ھ� �ؽ�Ʈ�� �Ҵ���� �ʽ��ϴ�!");

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
        UpdateTimerDisplay();

        // ���ھ� �ʱ�ȭ
        currentScore = 0;
        UpdateTimerDisplay();

        // UI �г� ����
        if (gamePanel != null)
            gamePanel.SetActive(true);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (timeChangeText != null)
            timeChangeText.gameObject.SetActive(true);

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
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerSlider.value = timeRemaining;
            UpdateTimerDisplay();
        }
        else
        {
            timeRemaining = 0;
            timerSlider.value = 0;
            UpdateTimerDisplay();
            EndGame();
        }
    }

    // Ÿ�̸� ǥ�� ������Ʈ
    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int second = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("[0:00]:[1:00]", minutes, second);
        }
    }

    // �ð� ���� (����/���п� ���� ����/�г�Ƽ)
    public void ChangeTime(int diceValue, bool isSuccess)
    {
        // �迭 �ε����� 0���� �����ϹǷ� ����
        int index = Mathf.Clamp(diceValue - 1, 0, 5);

        int timeChange = 0;
        if (isSuccess)
        {
            timeChange = timeRewardsByDice[index];
            timeRemaining += timeChange;
        }
        else
        {
            timeChange = -timePenaltiesByDice[index];
            timeRemaining = Mathf.Max(0, timeRemaining + timeChange); // ���� ����
        }

        // Ÿ�̸� �����̴� ������Ʈ
        timerSlider.value = timeRemaining;
        UpdateTimerDisplay();

        // �ð� ���� �˸� ǥ��
        ShowTimeChangeNotification(timeChange);
    }

    // �ð� ���� �˸� ǥ��
    void ShowTimeChangeNotification(int timeChange)
    {
        if (timeChangeText != null)
        {
            // ����� �ʷϻ�, ������ ������
            timeChangeText.color = timeChange >= 0 ? Color.green : Color.red;
            timeChangeText.text = (timeChange >= 0 ? "+" : "") + timeChange.ToString() + "��";
            timeChangeText.gameObject.SetActive(true);

            // �˸� �ڵ� ����
            StartCoroutine(HideTimeChangeNotification());
        }
    }

    // �˸� �ڵ� ����
    IEnumerator HideTimeChangeNotification()
    {
        yield return new WaitForSeconds(1.5f);
        if (timeChangeText != null)
            timeChangeText.gameObject.SetActive(false);
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
            scoreText.text = "Score: " + currentScore.ToString();
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
        Debug.Log("���� ����! ���� ���ھ�: " + currentScore);

        // UI �г� ��ȯ
        if (gamePanel != null)
            gamePanel.SetActive(false);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = "���� ����: " + currentScore.ToString();
        }

        // ���� �Ͻ����� (���� ����)
        Time.timeScale = 0;
    }

    // ����� ��ư (���� ���� �гο��� ���)
    public void RestartGame()
    {
        // ���� �ð� ����
        Time.timeScale = 1;

        // ���� �ٽ� �ʱ�ȭ
        InitializeGame();
    }
}
