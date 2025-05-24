using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("타이머 설정")]
    public Slider timerSlider;          // 타이머 슬라이더
    public float gameTime = 120f;       // 게임 시간 (2분 = 120초)
    private float timeRemaining;        // 남은 시간
    private bool isGameActive = false;  // 게임 활성화 상태

    [Header("스코어 설정")]
    public Text scoreText;   // 스코어 텍스트
    private int currentScore = 0;       // 현재 스코어

    [Header("UI 패널")]
    public GameObject gameOverPanel;    // 게임 종료 UI

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

   [Header("닉네임 저장")]
    public TMP_InputField nicknameInputField; 

    void Start()
    {
        // 컴포넌트 확인
        if (timerSlider == null)
            Debug.LogError("타이머 슬라이더가 할당되지 않았습니다!");
        if (scoreText == null)
            Debug.LogError("스코어 텍스트가 할당되지 않았습니다!");

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

        // 스코어 초기화
        currentScore = 0;
        UpdateScoreDisplay();

        // UI 패널 설정
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

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
        // 피버 타임 중엔 타이머 멈춤
        if (feverSystem != null && feverSystem.IsInFever())
        {
            return; // 피버 타임 중엔 시간이 흐르지 않게
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

    // 시간 변경 (성공/실패에 따른 보상/패널티)
    public void ChangeTime(int diceValue, bool isSuccess)
    {
        // 피버 타임 중엔 타이머 멈춤
        if (feverSystem != null && feverSystem.IsInFever())
        {
            return; // 피버 타임 중엔 시간이 흐르지 않게
        }

        // 배열 인덱스는 0부터 시작하므로 보정
        int index = Mathf.Clamp(diceValue - 1, 0, 5);

        if (isSuccess)
        {
            // 성공 시 - 시간 추가
            int timeReward = timeRewardsByDice[index];
            timeRemaining += timeReward;

            Debug.Log("성공! 시간 보상: +" + timeReward + "초 (현재 시간: " + timeRemaining + ")");
        }
        else
        {
            // 실패 시 - 시간 즉시 감소 (패널티)
            int timePenalty = timePenaltiesByDice[index];

            // 변경 전 값 저장
            float beforeChange = timeRemaining;

            // 시간 감소 적용
            timeRemaining -= timePenalty;
            timeRemaining = Mathf.Max(0, timeRemaining); // 음수 방지

            Debug.Log("실패! 시간 패널티: -" + timePenalty + "초 (" + beforeChange + " → " + timeRemaining + ")");
        }

        // 슬라이더 강제 업데이트
        if (timerSlider != null)
        {
            // 슬라이더 값을 즉시 업데이트
            timerSlider.value = timeRemaining;

            // UI 즉시 갱신을 위한 추가 코드
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)timerSlider.transform);

            Debug.Log("타이머 슬라이더 값 설정: " + timerSlider.value);
        }
        else
        {
            Debug.LogError("타이머 슬라이더가 null입니다!");
        }

        // Time.timeScale이 0인지 확인 (일시정지 상태인지)
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            Debug.LogWarning("주의: Time.timeScale이 0입니다. 타이머 업데이트가 제대로 작동하지 않을 수 있습니다!");
        }
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
            scoreText.text = currentScore.ToString();
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
       
         // 현재 점수를 ScoreManager에 저장
    ScoreManager.Instance.currentScore = currentScore;
    // 랭킹 닉네임 저장
    string nickname = PlayerPrefs.GetString("PlayerNickname", "Unknown");
    
    RankingManager.Instance.AddRank(nickname, currentScore);

        Debug.Log("게임 종료! 최종 스코어: " + currentScore);

        // 게임 오버 패널 활성화
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
           
        }

        // 게임 일시정지 (선택 사항)
        Time.timeScale = 1;

     
    }

    // 재시작 버튼 (게임 오버 패널에서 사용)
    public void RestartGame()
    {
        // 게임 시간 복원
        Time.timeScale = 1;

        // 게임 다시 초기화
        InitializeGame();
    }
  

public void SavePlayerNickname()
{
    string playerNickname = nicknameInputField.text;
    if (!string.IsNullOrEmpty(playerNickname))
    {
        PlayerPrefs.SetString("PlayerNickname", playerNickname);
        PlayerPrefs.Save();
        Debug.Log("닉네임 저장 완료: " + playerNickname);
    }
    else
    {
        Debug.LogWarning("닉네임이 비어있습니다.");
    }
}


}


