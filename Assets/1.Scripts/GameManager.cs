using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("타이머 설정")]
    public Slider timerSlider;          // 타이머 슬라이더
    public Text timerText;   // 타이머 텍스트 (추가)
    public float gameTime = 120f;       // 게임 시간 (2분 = 120초)
    private float timeRemaining;        // 남은 시간
    private bool isGameActive = false;  // 게임 활성화 상태

    [Header("스코어 설정")]
    public Text scoreText;   // 스코어 텍스트
    public Text finalScoreText; // 최종 스코어 텍스트 (게임 종료 시)
    private int currentScore = 0;       // 현재 스코어

    [Header("UI 패널")]
    public GameObject gamePanel;        // 게임 중 UI
    public GameObject gameOverPanel;    // 게임 종료 UI
    public Text timeChangeText; // 시간 변경 알림 텍스트 (추가)

    [Header("관련 시스템")]
    public RecipeManager recipeManager; // 레시피 매니저
    public FeverSystem feverSystem;     // 피버 시스템
    public RollDice diceRoller;         // 주사위 시스템

    [Header("보상/패널티 설정")]
    // 주사위 눈에 따른 성공 시 시간 보상 (인덱스 0 = 눈 1)
    private int[] timeRewardsByDice = { 3, 4, 6, 8, 10, 12 };
    // 주사위 눈에 따른 실패 시 시간 패널티
    private int[] timePenaltiesByDice = { 2, 3, 4, 5, 6, 7 };
    // 주사위 눈에 따른 성공 시 점수 보상
    private int[] scoreRewardsByDice = { 10, 20, 30, 40, 50, 60 };
    // 주사위 눈에 따른 실패 시 점수 패널티
    private int[] scorePenaltiesByDice = { 5, 10, 15, 20, 25, 30 };
    
    void Start()
    {
        // 컴포넌트 확인
        if (timerSlider == null)
            Debug.LogError("타이머 슬라이더 할당되지 않습니다!");
        if (scoreText == null)
            Debug.LogError("스코어 텍스트가 할당되지 않습니다!");

        // 게임 시작 설정
        InitializeGame();
    }

    void Update()
    {
        if (isGameActive)
        {
            UpdateTimer();
        }
    }

    // 게임 초기화
    public void InitializeGame()
    {
        // 타이머 초기화
        timeRemaining = gameTime;
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;
        UpdateTimerDisplay();

        // 스코어 초기화
        currentScore = 0;
        UpdateTimerDisplay();

        // UI 패널 설정
        if (gamePanel != null)
            gamePanel.SetActive(true);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (timeChangeText != null)
            timeChangeText.gameObject.SetActive(true);

        // 게임 시작
        isGameActive = true;

        // 필요시 연관된 시스템들 초기화
        if (recipeManager != null)
            Debug.Log("레시피 매니저 준비 완료");
        if (feverSystem != null)
            Debug.Log("피버 시스템 준비 완료");
        if (diceRoller != null)
            Debug.Log("주사위 시스템 준비 완료");
    }

    // 타이머 업데이트
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

    // 타이머 표시 업데이트
    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int second = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("[0:00]:[1:00]", minutes, second);
        }
    }

    // 시간 변경 (성공/실패에 따른 보상/패널티)
    public void ChangeTime(int diceValue, bool isSuccess)
    {
        // 배열 인덱스는 0부터 시작하므로 보정
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
            timeRemaining = Mathf.Max(0, timeRemaining + timeChange); // 음수 방지
        }

        // 타이머 슬라이더 업데이트
        timerSlider.value = timeRemaining;
        UpdateTimerDisplay();

        // 시간 변경 알림 표시
        ShowTimeChangeNotification(timeChange);
    }

    // 시간 변경 알림 표시
    void ShowTimeChangeNotification(int timeChange)
    {
        if (timeChangeText != null)
        {
            // 양수면 초록색, 음수면 빨간색
            timeChangeText.color = timeChange >= 0 ? Color.green : Color.red;
            timeChangeText.text = (timeChange >= 0 ? "+" : "") + timeChange.ToString() + "초";
            timeChangeText.gameObject.SetActive(true);

            // 알림 자동 숨김
            StartCoroutine(HideTimeChangeNotification());
        }
    }

    // 알림 자동 숨김
    IEnumerator HideTimeChangeNotification()
    {
        yield return new WaitForSeconds(1.5f);
        if (timeChangeText != null)
            timeChangeText.gameObject.SetActive(false);
    }

    // 스코어 업데이트 (성공/실패에 따른 보상/패널티)
    public void UpdateScore(int diceValue, bool isSuccess)
    {
        // 배열 인덱스는 0부터 시작하므로 보정
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

        // 점수 업데이트 (최소 0)
        currentScore = Mathf.Max(0, currentScore + scoreChange);
        UpdateScoreDisplay();

        // 점수 변경 로그
        Debug.Log((isSuccess ? "성공" : "실패") +
                 $" - 주사위 {diceValue}: 점수 {(scoreChange >= 0 ? "+" : "")}{scoreChange}" +
                 $" (현재 점수: {currentScore})");
    }
    // 스코어 표시 업데이트
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    // 햄버거 완성/실패 처리
    public void OnBurgerResult(int diceValue, bool isSuccess)
    {
        // 점수 업데이트
        UpdateScore(diceValue, isSuccess);

        // 시간 변경
        ChangeTime(diceValue, isSuccess);
    }

    // 게임 종료
    void EndGame()
    {
        if (!isGameActive)
            return;

        isGameActive = false;
        Debug.Log("게임 종료! 최종 스코어: " + currentScore);

        // UI 패널 전환
        if (gamePanel != null)
            gamePanel.SetActive(false);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = "최종 점수: " + currentScore.ToString();
        }

        // 게임 일시정지 (선택 사항)
        Time.timeScale = 0;
    }

    // 재시작 버튼 (게임 오버 패널에서 사용)
    public void RestartGame()
    {
        // 게임 시간 복원
        Time.timeScale = 1;

        // 게임 다시 초기화
        InitializeGame();
    }
}
