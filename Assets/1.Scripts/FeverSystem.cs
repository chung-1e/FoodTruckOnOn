using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverSystem : MonoBehaviour
{
    [Header("UI 요소")]
    public Image frameImage;    // 큰 툴 이미지
    public Image[] streakImages;  // 5개의 이미지 배열 (0이 아래, 4가 위)
    public GameObject feverPanel;   // 피버 타임 패널

    [Header("이미지 스트라이트")]
    public Sprite[] grayscaleSprites;  // 서로 다른 5개의 흑백 이미지
    public Sprite[] colorSprites;  // 서로 다른 5개의 컬러 이미지

    [Header("피버 타임 설정")]
    public float feverDuration = 10f;   // 피버 타임 지속시간: 10초
    public float feverPanelDuration = 1f; // 피버 패널 표시 시간: 1초
    public float autoBuildCooldown = 0.1f; // 연타 방지 쿨다운 시간 (짧게 설정)

    [Header("필요 컴포넌트 할당")]
    public CookingStation cookingStation;
    public RecipeManager recipeManager;
    public RollDice diceRoller;

    private int currentStreak = 0;  // 현재 연속 성공 횟수
    private bool isInFever = false; // 피버 타임 중인지
    private bool isProcessingHamburger = false; // 햄버거 처리 중 상태 추가
    private float lastBuildTime = 0f; // 마지막으로 햄버거를 만든 시간

    void Start()
    {
        // 초기화 - 모든 이미지를 흑백으로 설정
        ResetStreak();

        // 피버 패널 비활성화
        if (feverPanel != null )
        {
            feverPanel.SetActive(false);
        }

        // 필요한 컴포넌트들을 찾아서 할당
        if (cookingStation == null)
            cookingStation = FindObjectOfType<CookingStation>();
        if (recipeManager == null)
            recipeManager = FindObjectOfType<RecipeManager>();
        if (diceRoller == null)
            diceRoller = FindObjectOfType<RollDice>();

        Debug.Log("FeverSystem 시작됨: streakImages 길이 = " + streakImages.Length);
    }

    void Update()
    {
        // 단, 햄버거 처리 중일 때는 무시
        // 피버 타임 중이고 스페이스바를 누른 경우, 자동으로 재료 쌓기
        if (isInFever && Input.GetKeyDown(KeyCode.Space))
        {
            // 쿨다운 시간 확인 (마지막 빌드 후 일정 시간이 지났는지)
            float currentTime = Time.time;
            if (currentTime - lastBuildTime >= autoBuildCooldown && !isProcessingHamburger)
            {
                if (cookingStation != null && recipeManager != null)
                {
                    Debug.Log("피버 타임 중 스페이스바 감지 - 자동 햄버거 제작");
                    AutoBuildBurger();
                    lastBuildTime = currentTime; // 마지막 빌드 시간 업데이트
                }
            }
            else
            {
                Debug.Log($"쿨다운 중 ({autoBuildCooldown - (currentTime - lastBuildTime)}초 남음)");
            }
        }
    }

    // 햄버거를 자동으로 만드는 함수
    public void AutoBuildBurger()
    {
        if (!isInFever || cookingStation == null || recipeManager == null) return;

        // 이미 처리 중이면 중복 실행 방지
        if (isProcessingHamburger)
        {
            Debug.Log("이미 햄버거 처리 중 - 요청 무시");
            return;
        }

        // 햄버거 처리 시작 상태로 전환
        isProcessingHamburger = true;
        Debug.Log("햄버거 자동 제작 시작 - 처리 중 상태로 전환");

        // 중요: 먼저 조리대를 초기화해야 함
        cookingStation.ClearAllFoods();

        // 레시피에 따라 재료를 자동으로 쌓기
        List<string> recipe = recipeManager.GetCurrentRecipe();
        foreach (string ingredientName in recipe)
        {
            GameObject newIngredient = CreateIngredientByName(ingredientName);
            if (newIngredient != null)
            {
                cookingStation.AddIngredient(newIngredient);
            }
        }

        // 레시피 완성 확인 - 스트릭 증가 없이 확인만 수행
        StartCoroutine(CompleteRecipeAfterDelay(0.01f)); // 약간의 딜레이 후 완성 처리
    }

    // 약간의 딜레이 후 레시피 완성 처리 (모든 재료가 쌓이도록)
    IEnumerator CompleteRecipeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        recipeManager.CheckRecipeCompletion();

        // 다음 주사위가 굴러간 후 처리 상태 해제
        StartCoroutine(ResetProcessingState());
    }

    // 햄버거 처리 상태를 초기화하는 코루틴
    IEnumerator ResetProcessingState()
    {
        // 짧은 딜레이 후 처리 상태 해제 (다음 주사위가 굴러간 후)
        yield return new WaitForSeconds(0.3f);
        isProcessingHamburger = false;
        Debug.Log("햄버거 처리 상태 초기화 완료 - 다음 입력 가능");
    }

    // 재료 이름에 따라 새 재료를 생성하는 함수
    private GameObject CreateIngredientByName(string ingredientName)
    {
        IngredientStation[] stations = FindObjectsOfType<IngredientStation>();
        foreach (IngredientStation station in stations)
        {
            if (station.GetIngredientName() == ingredientName ||
                (ingredientName == "아래 빵" && station.stationType == IngredientStation.IngredientType.Bread) ||
                (ingredientName == "위 빵" && station.stationType == IngredientStation.IngredientType.Bread))
            {
                return station.SpawnIngredient();
            }
        }
        return null;
    }

    // 햄버거 완성 시 호출
    public void OnBurgerCompleted()
    {
        if (isInFever)
        {
            Debug.Log("피버 타임 중 성공은 스트릭에 영향 없음");
            return;
        }

        currentStreak++;
        Debug.Log($"햄버거 성공! 현재 스트릭: {currentStreak}/{streakImages.Length}");
        UpdateStreakDisplay();

        if (currentStreak >= streakImages.Length)
        {
            StartFever();
        }
    }

    // 실패 시 호출
    public void OnBurgerFailed()
    {
        if (!isInFever)
        {
            Debug.Log("햄버거 실패! 콤보 게이지 초기화");
            ResetStreak();
        }
        else
        {
            // 혹시라도 버그가 발생할 것을 대비하여
            Debug.Log("피버 타임 중 실패는 콤보 게이지 영향 없음");
        }
    }

    // 연속 성공 디스플레이 업데이트
    private void UpdateStreakDisplay()
    {
        if (streakImages == null || streakImages.Length == 0)
        {
            Debug.LogError("스트릭 이미지 배열이 없거나 비어있습니다!");
            return;
        }

        if (grayscaleSprites == null || grayscaleSprites.Length == 0 ||
            colorSprites == null || colorSprites.Length == 0)
        {
            Debug.LogError("스프라이트 배열이 없거나 비어있습니다!");
            return;
        }

        Debug.Log($"스트릭 디스플레이 업데이트 - 현재 스트릭: {currentStreak}");

        for (int i = 0; i < streakImages.Length; i++)
        {
            if (streakImages[i] == null)
            {
                Debug.LogError($"스트릭 이미지 {i}번이 null입니다!");
                continue;
            }

            if (i < currentStreak)
            {
                if (i < colorSprites.Length)
                {
                    streakImages[i].sprite = colorSprites[i];
                    Debug.Log($"이미지 {i}: 컬러 이미지로 설정");
                }
            }
            else
            {
                if (i < grayscaleSprites.Length)
                {
                    streakImages[i].sprite = grayscaleSprites[i];
                    Debug.Log($"이미지 {i}: 흑백 이미지로 설정");
                }
            }
        }
    }

    // 피버 타임 시작
    private void StartFever()
    {
        isInFever = true;
        Debug.Log("피버 타임 시작!");

        if (feverPanel != null)
        {
            feverPanel.SetActive(true);
            // 패널은 1초만 표시하고 숨기지만, 피버 상태는 계속 유지
            StartCoroutine(HideFeverPanel());
        }

        // 피버 타임은 10초 동안 유지
        StartCoroutine(FeverTimer());
    }

    IEnumerator HideFeverPanel()
    {
        yield return new WaitForSeconds(feverPanelDuration);
        if (feverPanel != null)
        {
            feverPanel.SetActive(false);
            Debug.Log(" 피버 패널 숨김 (피버 타임은 계속 유지)");
        }
    }

    // 피버 타임 타이머
    IEnumerator FeverTimer()
    {
        yield return new WaitForSeconds(feverDuration);
        EndFever();
    }

    // 피버 타임 종료
    private void EndFever()
    {
        isInFever = false;
        if (feverPanel != null)
        {
            feverPanel.SetActive(false);
        }
        ResetStreak();
    }

    // 연속 성공 리셋
    private void ResetStreak()
    {
        Debug.Log($"스트릭 초기화: {currentStreak} -> 0");
        currentStreak = 0;
        UpdateStreakDisplay();
    }

    // 현재 피버 타임인지 확인
    public bool IsInFever()
    {
        return isInFever;
    }

}
