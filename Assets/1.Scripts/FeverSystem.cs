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

    // 추가
    [Header("필요 컴포넌트 할당")]
    public CookingStation cookingStation;
    public RecipeManager recipeManager;
    public RollDice diceRoller;

    private int currentStreak = 0;  // 현재 연속 성공 횟수
    private bool isInFever = false; // 피버 타임 중인지

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
    }

    private void Update()
    {
        // 피버 타임 중이고 스페이스바를 누른 경우, 자동으로 재료 쌓기
        if (isInFever && Input.GetKeyDown(KeyCode.Space))
        {
            if(cookingStation != null && recipeManager != null)
            {
                AutoBuildBurger();
            }
        }
    }

    // 햄버거를 자동으로 만드는 향수
    public void AutoBuildBurger()
    {
        if (!isInFever || cookingStation == null || recipeManager == null) return;

        // 현재 조리대 초기화
        cookingStation.ClearAllFoods();

        // 레시피레 따라 재료들 자동으로 쌓기
        List<string> recipe = recipeManager.GetCurrentRecipe();
        foreach (string ingredientName in recipe)
        {
            GameObject newIngredient = CreateIngredientByName(ingredientName);
            if (newIngredient != null )
            {
                cookingStation.AddIngredient(newIngredient);
            }
        }

        // 레시피 완성 화면 - 콤보 게이지 증가 없이 확인만 수행
        recipeManager.CheckRecipeCompletion();
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
        if (isInFever) return;

        currentStreak++;
        UpdateStreakDiplay();

        if (currentStreak >= streakImages.Length)
        {
            StartFever();
        }
    }

    // 실패 시 호출
    public void OnBurgerFailed()
    {
        if (isInFever)
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
    private void UpdateStreakDiplay()
    {
        for (int i = 0; i < streakImages.Length; i++)
        {
            if (i < currentStreak)
            {
                // 성공한 만큼 컬러 이미지로 변경
                streakImages[i].sprite = colorSprites[i];
            }
            else
            {
                // 아직 달성하지 못한 것도 흑백으로 유지
                streakImages[i].sprite = grayscaleSprites[i];
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
        currentStreak = 0;
        UpdateStreakDiplay();
    }

    // 현재 피버 타임인지 확인
    public bool IsInFever()
    {
        return isInFever;
    }

}
