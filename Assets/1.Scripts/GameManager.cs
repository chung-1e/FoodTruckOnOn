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
    private float playTime = 0f;        //플레이 시간
    private bool isGameActive = false;  // 게임 활성화 상태
    private float soundPlayTime = 5f; // 사운드 재생 시간

    [Header("카운트다운")]
    public GameObject countdownPanel;     // 카운트다운 보여줄 패널
    public TMP_Text countdownText;        // 숫자 텍스트 

    [Header("스코어 설정")]
    public Text scoreText;   // 스코어 텍스트
    public TMP_Text finalScoreText; // 최종점수 텍스트
    private int currentScore = 0;       // 현재 스코어

    [Header("UI 패널")]
    public GameObject gameOverPanel;    // 게임 종료 UI
    public Button rankingSubmitButton; // 랭킹 등록 버튼
    private bool isRankingSubmitted = false;  //랭킹 등록 여부
    
    
    [Header("관련 시스템")]
    public RecipeManager recipeManager;  // 레시피 매니저
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

    [Header("사운드")]
    public string timeWarningSFXName = "시간 임박";
    public string countdownSFXName = "카운트다운";
    public string timeOver1SFXName = "타임 오버";
    public string timeOver2SFXName = "타임 오버(점수 미달)";

    private bool isTimeWarningPlayed = false;
    private bool isTimeOver1Played = false;
    private bool isTimeOver2Played = false;

     private Coroutine countdownCoroutine;
    void Start()
    {
        // 컴포넌트 확인
        if (timerSlider == null)
            Debug.LogError("타이머 슬라이더가 할당되지 않았습니다!");
        if (scoreText == null)
            Debug.LogError("스코어 텍스트가 할당되지 않았습니다!");

        // 게임 시작 설정
        InitializeGame();

  if (countdownCoroutine == null)
        countdownCoroutine = StartCoroutine(StartCountdown());
    }

    void Update()
    {
        if (isGameActive)
        {
            UpdateTimer();
            UpdatePlayTime();
        }

    }

    // 게임 초기화
    public void InitializeGame()
    {
        // 타이머 초기화
        timeRemaining = gameTime;
        timerSlider.maxValue = gameTime;
        timerSlider.value = gameTime;

        playTime = 0f;
        
        // 스코어 초기화
        currentScore = 0;
        UpdateScoreDisplay();
       
        // 닉네임 InputField 초기화
        if (nicknameInputField != null)
        {
            nicknameInputField.text = "";
        }
        
        // UI 패널 설정
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 게임 시작
        isGameActive = false;

        // 필요시 연관된 시스템들 초기화
        if (recipeManager != null)
            Debug.Log("레시피 매니저 준비 완료");
        if (feverSystem != null)
            Debug.Log("피버 시스템 준비 완료");
        if (diceRoller != null)
            Debug.Log("주사위 시스템 준비 완료");

        // 시간 정상화
        Time.timeScale = 1f;
        soundPlayTime = 5f;

         isTimeWarningPlayed = false; // 초기화 시점에 false로
        isTimeOver1Played = false;
        isTimeOver2Played = false;
    }

    // 타이머 업데이트
    void UpdateTimer()
{
    // 피버 타임 중엔 타이머 멈춤
    if (feverSystem != null && feverSystem.IsInFever())
    {
        return;
    }

    if (timeRemaining > 0)
    {
        timeRemaining -= Time.deltaTime;
        timerSlider.value = timeRemaining;

        // 시간 임박 사운드 체크
        if (timeRemaining <= 5f)
        {
            if (!isTimeWarningPlayed)
            {
                AudioManager.Instance.PlaySFXOver(timeWarningSFXName);
                isTimeWarningPlayed = true;
            }
        }
        else
        {
            // 5초 이상이면 플래그 리셋
            isTimeWarningPlayed = false;
        }
    }
    else
    {
        timeRemaining = 0;
        timerSlider.value = 0;
        EndGame();
    }
}


    void UpdatePlayTime()
    {
        playTime += Time.deltaTime;
    }

    public float GetPlayTime()
    {
        return playTime;
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

            Debug.Log($"타이머 슬라이더 값 설정: {timerSlider.value:F1}");
        }

        // 게임 중지 상태인지를 체크하기 위한
        if (timeRemaining <= 0 && isGameActive)
        {
            EndGame();
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
        isRankingSubmitted = false;
       
        Debug.Log("게임 종료! 최종 스코어: " + currentScore);
        
        // 현재 점수를 ScoreManager에 저장
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.currentScore = currentScore;
            ScoreManager.Instance.playTime = playTime;
        }

        
        // 게임 오버 패널 활성화
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = "최종점수: " + currentScore.ToString();
        }

        soundPlayTime -= Time.deltaTime;
        // TimeOver 사운드 재생
        if (currentScore > 400)
        {
            if (soundPlayTime != 0)
            {
                if (!isTimeOver1Played)
                {
                    AudioManager.Instance.PlaySFXOver(timeOver1SFXName);
                    isTimeOver1Played = true;
                    isTimeWarningPlayed = false;
                    AudioManager.Instance.PlaySceneMusic("");
                }
            }
            else
            {
                isTimeOver1Played = false;
                AudioManager.Instance.PlaySceneMusic("IngameScene");
            }
        }
        else
        {
            if (soundPlayTime != 0)
            {
                if (!isTimeOver2Played)
                {
                    AudioManager.Instance.PlaySFXOver(timeOver2SFXName);
                    isTimeOver2Played = true;
                    isTimeWarningPlayed = false;
                    AudioManager.Instance.PlaySceneMusic("");
                }
            }
            else
            {
                isTimeOver1Played = false;
                AudioManager.Instance.PlaySceneMusic("IngameScene");
            }
        }
    }
    
   
    //랭킹 등록 버튼 클릭 시 호출되는 매서드
    public void SubmitRanking()
    {
        if (isRankingSubmitted)
        {
            return;
        }
    
    
        // 닉네임 저장
         SavePlayerNickname();

         // 닉네임 가져오기
         string nickname = "";
    
         if (nicknameInputField != null)
         {
            nickname = nicknameInputField.text.Trim();
         }

         if(string.IsNullOrEmpty(nickname) || string.IsNullOrWhiteSpace(nickname))
         {
           nickname = "Unknown";
         }
          
         if (nicknameInputField != null && !string.IsNullOrEmpty
                (nicknameInputField.text.Trim()))
                {
                    PlayerPrefs.SetString("playerNickname", nickname);
                    PlayerPrefs.Save();
                }

        //랭킹 등록은 NicknameValidator에서만 처리하도록 제거
         isRankingSubmitted = true;

            //중복 등록 방지를 위해 버튼 비활성화
            if (rankingSubmitButton != null)
            {
                rankingSubmitButton.interactable = false;
            }
        
         
    }
    // 재시작 버튼 (게임 오버 패널에서 사용)
    public void RestartGame()
    {
        // 게임 시간 복원
        Time.timeScale = 1;

        // 게임 다시 초기화
        InitializeGame();
    }

    private string GetPlayerNickname()
    {
        // InputField에서 1차 확인
        if (nicknameInputField != null && !string.IsNullOrEmpty(nicknameInputField.text))
        {
            return nicknameInputField.text.Trim();
        }

        // PlayerPrefs에서 2차 확인
        string savedNickname = PlayerPrefs.GetString("PlayerNickname", "");
        if (!string.IsNullOrEmpty(savedNickname))
        {
            return savedNickname;
        }

        // 아무것도 입력하지 않았을때 기본값
        return "Unknown";
    }

    public void SavePlayerNickname()
    {
        if (nicknameInputField == null)
        {
            return;
        }

        string playerNickname = nicknameInputField.text.Trim();
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

private IEnumerator StartCountdown()
{
    if (countdownPanel != null) countdownPanel.SetActive(true);

    int count = 3;
    while (count > 0)
    {
        if (countdownText != null)
            countdownText.text = count.ToString();

        AudioManager.Instance.PlaySFX(countdownSFXName);

        yield return new WaitForSeconds(1f);
        count--;
    }

    if (countdownText != null)
        countdownText.text = "시작!";

    yield return new WaitForSeconds(1f);

    if (countdownPanel != null)
        countdownPanel.SetActive(false);

    isGameActive = true;

    //  카운트다운 끝난 후 주사위 굴림 실행
    if (diceRoller != null)
        diceRoller.Rolling();
}

}


