using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("타이머 설정")]
    public Slider timerSlider;
    public float gameTime = 120f;
    private float timeRemaining;
    private bool isGameActive = false;

    [Header("스코어 설정")]
    public Text scoreText;
    public Text finalScoreText;
    private int currentScore = 0;

    [Header("UI 패널")]
    public GameObject gameOverPanel;

    [Header("관련 시스템")]
    public RecipeManager recipeManager;
    public FeverSystem feverSystem;
    public RollDice diceRoller;

    [Header("보상/패널티 설정")]
    private int[] timeRewardsByDice = { 3, 4, 6, 8, 10, 12 };
    private int[] timePenaltiesByDice = { 2, 3, 4, 5, 6, 7 };
    private int[] scoreRewardsByDice = { 10, 20, 30, 40, 50, 60 };
    private int[] scorePenaltiesByDice = { 5, 10, 15, 20, 25, 30 };

    void Start()
    {
        if (timerSlider == null)
            Debug.LogError("타이머 슬라이더가 할당되지 않았습니다!");
        if (scoreText == null)
            Debug.LogError("스코어 텍스트가 할당되지 않았습니다!");

        InitializeGame();
    }

    void Update()
    {
        if (isGameActive)
        {
            UpdateTimer();
        }
    }

    public void InitializeGame()
    {
        timeRemaining = gameTime;
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;

        currentScore = 0;
        UpdateScoreDisplay();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        isGameActive = true;

        if (recipeManager != null)
            Debug.Log("레시피 매니저 준비 완료");
        if (feverSystem != null)
            Debug.Log("피버 시스템 준비 완료");
        if (diceRoller != null)
            Debug.Log("주사위 시스템 준비 완료");

        Time.timeScale = 1; // 초기화 시 타임스케일 복원
    }

    void UpdateTimer()
    {
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

    public void ChangeTime(int diceValue, bool isSuccess)
    {
        int index = Mathf.Clamp(diceValue - 1, 0, 5);

        if (isSuccess)
        {
            int timeReward = timeRewardsByDice[index];
            timeRemaining += timeReward;
            Debug.Log("성공! 시간 보상: +" + timeReward + "초 (현재 시간: " + timeRemaining + ")");
        }
        else
        {
            int timePenalty = timePenaltiesByDice[index];
            float beforeChange = timeRemaining;
            timeRemaining -= timePenalty;
            timeRemaining = Mathf.Max(0, timeRemaining);
            Debug.Log("실패! 시간 패널티: -" + timePenalty + "초 (" + beforeChange + " → " + timeRemaining + ")");
        }

        if (timerSlider != null)
        {
            timerSlider.value = timeRemaining;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)timerSlider.transform);
        }
        else
        {
            Debug.LogError("타이머 슬라이더가 null입니다!");
        }

        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            Debug.LogWarning("주의: Time.timeScale이 0입니다. 타이머 업데이트가 제대로 작동하지 않을 수 있습니다!");
        }
    }

    public void UpdateScore(int diceValue, bool isSuccess)
    {
        int index = Mathf.Clamp(diceValue - 1, 0, 5);
        int scoreChange = isSuccess ? scoreRewardsByDice[index] : -scorePenaltiesByDice[index];

        currentScore = Mathf.Max(0, currentScore + scoreChange);
        UpdateScoreDisplay();

        Debug.Log((isSuccess ? "성공" : "실패") +
                  $" - 주사위 {diceValue}: 점수 {(scoreChange >= 0 ? "+" : "")}{scoreChange}" +
                  $" (현재 점수: {currentScore})");
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public void OnBurgerResult(int diceValue, bool isSuccess)
    {
        UpdateScore(diceValue, isSuccess);
        ChangeTime(diceValue, isSuccess);
    }

    void EndGame()
    {
        if (!isGameActive)
            return;

        isGameActive = false;
        Debug.Log("게임 종료! 최종 스코어: " + currentScore);

        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        if (gameOverUI != null)
        {
            gameOverUI.SetScore(currentScore);
        }
        else
        {
            Debug.LogWarning("GameOverUI 오브젝트를 찾을 수 없습니다.");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = "게임 종료!\n최종 점수: " + currentScore.ToString();
        }

        Time.timeScale = 0;
        StartCoroutine(LoadResultSceneAfterDelay());
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        InitializeGame();
    }

   IEnumerator LoadResultSceneAfterDelay()
{
    yield return new WaitForSecondsRealtime(2f);
    Time.timeScale = 1;
    SceneManager.LoadScene("Exit");

    yield return null; // 씬 전환 후 한 프레임 대기

    GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
    if (gameOverUI != null)
    {
        gameOverUI.SetScore(currentScore);
    }
}

}
