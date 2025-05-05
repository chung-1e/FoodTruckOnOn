using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    [Header("UI 요소")]
    public Text recipeDisplay; // 레시피를 표시할 Text 컴포넌트

    [Header("주사위")]
    public RollDice diceRoller; // 주사위 굴리기 컴포넌트

    [Header("조리대")]
    public CookingStation cookingStation;   // 조리대 참조

    [Header("완성 햄버거 설정")]
    public float burgerDisplayDuration = 2f;    // 완성 햄버거 표시 시간

    // 현재 레시피
    private List<string> currentRecipe = new List<string>();
    private int requiredMainIngredients = 0;    // 메인 재료의 개수
    private bool requiresFrenchFries = false;
    private bool requiresCola = false;

    // 재료 타입과 이름 매핑
    private Dictionary<IngredientStation.IngredientType, string> ingredientNames;

    //햄버거 완성 상태
    private bool isBurgerCompleted = false;
    private GameObject completedBurger = null;

    void Start()
    {
        InitializeIngredientNames();

        // 주사위 시스템과 연결
        if (diceRoller == null )
        {
            Debug.LogError("RollDice 컴포넌트가 할당되지 않았습니다!");
        }
    }

    void InitializeIngredientNames()
    {
        ingredientNames = new Dictionary<IngredientStation.IngredientType, string>
        {
            { IngredientStation.IngredientType.Bread, "빵" },
            { IngredientStation.IngredientType.Patty, "패티" },
            { IngredientStation.IngredientType.Lettuce, "양상추" },
            { IngredientStation.IngredientType.Tomato, "토마토" },
            { IngredientStation.IngredientType.Cheese, "치즈" },
            { IngredientStation.IngredientType.Shrimp, "새우" },    // 새우 추가
            { IngredientStation.IngredientType.Chicken, "치킨" },   // 치킨 추가
            { IngredientStation.IngredientType.FrenchFries, "감자튀김" },
            { IngredientStation.IngredientType.Cola, "콜라" }
        };
    }


    public void GenerateRecipe(int diceNumber)
    {
        currentRecipe.Clear();

        // 아래 빵
        currentRecipe.Add("아래 빵");

        // 메인 재료들 (주사위 눈 수만큼)
        requiredMainIngredients = diceNumber;
        for (int i = 0; i < diceNumber; i++)
        {
            // 무작위로 메인 재료 선택 (패티, 양상추, 토마토, 치즈, 새우, 치킨 중에서)
            IngredientStation.IngredientType[] mainIngredients = {
            IngredientStation.IngredientType.Patty,
            IngredientStation.IngredientType.Lettuce,
            IngredientStation.IngredientType.Tomato,
            IngredientStation.IngredientType.Cheese,
            IngredientStation.IngredientType.Shrimp,   // 새우 추가
            IngredientStation.IngredientType.Chicken   // 치킨 추가
    };

            int randomIndex = Random.Range(0, mainIngredients.Length);
            string ingredientName = ingredientNames[mainIngredients[randomIndex]];
            currentRecipe.Add(ingredientName);
        }

        // 위 빵
        currentRecipe.Add("위 빵");

        // 사이드 메뉴 - 기존 규칙 유지 (주사위 5: 감자튀김, 주사위 6: 감자튀김+콜라)
        requiresFrenchFries = (diceNumber >= 5);
        requiresCola = (diceNumber == 6);

        if (requiresFrenchFries)
        {
            currentRecipe.Add("감자튀김");
        }

        if (requiresCola)
        {
            currentRecipe.Add("콜라");
        }

        // 레시피 표시
        DisplayRecipe();
    }

    void DisplayRecipe()
    {
        if (recipeDisplay ==  null)
        {
            Debug.LogError("레시피 표시 Text 컴포넌트가 할당되지 않았습니다!");
            return;
        }

        string recipeText = "[ 레시피 ]\n\n";

        foreach (string ingredient in currentRecipe)
        {
            recipeText += ingredient + "\n";
        }

        recipeDisplay.text = recipeText;
    }

    public void CheckIngredientOrder()
    {
        List<string> currentIngredients = GetCurrentIngredientsInOrder();

        Debug.Log("현재 올린 재료: " + string.Join(", ", currentIngredients));
        Debug.Log("레시피 순서: " + string.Join(", ", currentRecipe));

        // 현재까지 올린 재료가 레시피 순서와 맞는지 확인
        for (int i = 0; i < currentIngredients.Count; i++)
        {
            // 레시피 범위를 벗어나거나 순서가 틀린 경우
            if (i >= currentRecipe.Count || currentIngredients[i] != currentRecipe[i])
            {
                Debug.Log($"잘못된 순서! 위치 {i}: 예상 '{(i < currentRecipe.Count ? currentRecipe[i] : "없음")}' 실제 '{currentIngredients[i]}'");
                cookingStation.ClearAllFoods();
                return;
            }
        }

        // 모든 재료가 레시피대로 올라갔는지 확인
        if (currentIngredients.Count == currentRecipe.Count)
        {
            Debug.Log("모든 재료가 올바른 순서로 올라갔습니다!");
            CheckRecipeCompletion();
        }
    }

    public void CheckRecipeCompletion()
    {
        // 현재 조리대에 있는 모든 재료 가져오기
        List<string> placedItems = GetPlacedItemsInOrder();

        //레시피와 비교
        if (IsRecipeCorrect(placedItems))
        {
            Debug.Log("완벽합니다!");
            // 여기에 성공 처리 코드 추가 가능
        }
        else
        {
            Debug.Log("잘못 만들었습니다.");
            cookingStation.ClearAllFoods();
        }
    }

    List<string> GetPlacedItemsInOrder()
    {
        List<string> placedItems = new List<string>();

        //CookingStation에서 재료 순서 가져오기
        foreach (GameObject ingredient in cookingStation.placedIngredients)
        {
            if (ingredient != null)
            {
                string name = ingredient.name;

                // "아래 빵"과 "위 빵"을 정확히 구분
                if (name.Contains("아래 빵") || name == "bread_bottom")
                {
                    placedItems.Add("아래 빵");
                }
                else if (name.Contains("위 빵") || name == "bread_top")
                {
                    placedItems.Add("위 빵");
                }
                else
                {
                    // 기타 재료들
                    Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
                    if (ingredientComponent != null && ingredientNames.ContainsKey(ingredientComponent.type))
                    {
                        placedItems.Add(ingredientNames[ingredientComponent.type]);
                    }
                }
            }
        }

        // 사이드 메뉴 확인
        if (cookingStation.sideMenus.ContainsKey(IngredientStation.IngredientType.FrenchFries))
        {
            placedItems.Add("감자튀김");
        }

        if (cookingStation.sideMenus.ContainsKey(IngredientStation.IngredientType.Cola))
        {
            placedItems.Add("콜라");
        }

        return placedItems;
    }

    bool IsRecipeCorrect(List<string> placedItems)
    {
        // 길이가 다르면 실패
        if (placedItems.Count != currentRecipe.Count)
        {
            return false;
        }

        // 각 항목이 순서대로 정확한지 확인
        for (int i = 0; i < currentRecipe.Count; i++)
        {
            if (placedItems[i] != currentRecipe[i])
            {
                return false;
            }
        }

        return true;
    }

    private void CreateCompletedBurger()
    {
        // 조리대 위 모든 재료 제거
        cookingStation.ClearAllFoods();

        // 햄버거 프리팹 생성
        Vector3 burgerPosition = cookingStation.transform.position + Vector3.up * 0.3f;
        completedBurger = Instantiate(cookingStation.hamburgerPrefab, burgerPosition, Quaternion.identity);
        completedBurger.name = "완성된 햄버거";

        isBurgerCompleted = true;
    }

    public void OnNewRecipeGenerated()
    {
        // 새로운 레시피가 생성될 때 이전 햄버거 제거
        if (completedBurger != null)
        {
            Destroy(completedBurger);
            completedBurger = null;
            isBurgerCompleted = false;
        }
    }

    private List<string> GetCurrentIngredientsInOrder()
    {
        List<string> currentIngredients = new List<string>();

        // 현재 조리대에 올라가 있는 햄버거 재료만 가져오기
        foreach (GameObject ingredient in cookingStation.placedIngredients)
        {
            if (ingredient != null)
            {
                string name = ingredient.name;

                if (name.Contains("아래 빵") || name == "bread_bottom")
                {
                    currentIngredients.Add("아래 빵");
                }
                else if (name.Contains("위 빵") || name == "bread_top")
                {
                    currentIngredients.Add("위 빵");
                }
                else
                {
                    Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
                    if (ingredientComponent != null && ingredientNames.ContainsKey(ingredientComponent.type))
                    {
                        currentIngredients.Add(ingredientNames[ingredientComponent.type]);
                    }
                }
            }
        }

        return currentIngredients;
    }
}