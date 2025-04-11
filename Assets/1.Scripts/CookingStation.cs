using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    // 햄버거 재료들
    private List<GameObject> placedIngredients = new List<GameObject>();

    private Dictionary<IngredientStation.IngredientType, GameObject> sideMenus
        = new Dictionary<IngredientStation.IngredientType, GameObject>();

    public float stackHeight = 0.2f;

    public GameObject hamburgerPrefab;

    // 사이드 메뉴가 놓일 위치
    public Transform rightSidePosition;
    public Transform leftSidePosition;

    public bool HasAnyIngredients()
    {
        return placedIngredients.Count > 0;
    }

    public void AddIngredient(GameObject ingredient)
    {
        if (ingredient == null)
            return;

        Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
        if (ingredientComponent == null)
            return;

        // 사이드 메뉴인지 확인
        if (ingredientComponent.isSideMenu)
        {
            AddSideMenu(ingredient, ingredientComponent.type);
        }
        else
        {
            // 햄버거 재료 추가
            placedIngredients.Add(ingredient);

            ingredient.transform.SetParent(transform);

            StackIngredients();

            // 햄버거 완성 여부 확인
            CheckBurgerCompletion();
        }
    }

    private void AddSideMenu(GameObject sideMenu, IngredientStation.IngredientType type)
    {
        // 이미 같은 종류의 사이드 메뉴가 있다면 제거
        if (sideMenus.ContainsKey(type))
        {
            Destroy(sideMenus[type]);
            sideMenus.Remove(type);
        }

        // 사이드 메뉴 배치 위치 설정
        Transform targetPosition;
        if (type == IngredientStation.IngredientType.FrenchFries)
        {
            targetPosition = leftSidePosition;
        }
        else if (type == IngredientStation.IngredientType.Cola)
        {
            targetPosition = rightSidePosition;
        }
        else
        {
            // 알 수 없는 사이드 메뉴가 있을 시 제거
            Debug.LogWarning("알 수 없는 사이드 메뉴 유형 : " + type);
            Destroy(sideMenu);
            return;
        }

        // 사이드 메뉴 위치 조정
        if (targetPosition != null)
        {
            sideMenu.transform.position = targetPosition.position;
            sideMenu.transform.SetParent(targetPosition);

            // 사이드 메뉴 등록
            sideMenus[type] = sideMenu;

            Debug.Log(type + "사이드 메뉴가 조리대 옆에 배치되었습니다.");
        }
        else
        {
            Debug.Log("사이드 메뉴 배치 위치가 설정되지 않았습니다.");
            Destroy(sideMenu);
        }
    }

    private void CheckBurgerCompletion()    // 햄버거 완성 여부 메서드
    {
        //아래 빵이 조리대에 없다 (기본값)
        bool hasBreadBottom = false;

        //위 빵이 조리대에 없다 (기본값)
        bool hasBreadTop = false;


        // 모든 재료를 검사하여 아래 빵과 위 빵이 있는지 확인
        foreach (GameObject ingredient in placedIngredients)
        {
            Ingredient ingredientComponenet = ingredient.GetComponent<Ingredient>();

            if (ingredientComponenet != null)
            {
                // 재료 이름으로 빵 종류 확인
                if (ingredient.name.Contains("아래 빵") || ingredient.name == "bread_bottom")
                {
                    hasBreadBottom = true;
                }
                else if (ingredient.name.Contains("위 빵") || ingredient.name == "bread_top")
                {
                    hasBreadTop = true;
                }
            }
        }

        // 양쪽 빵이 모두 있으면 햄버거 완성
        if (hasBreadBottom && hasBreadTop)
        {
            CompleteBuger();
        }
    }

    private void CompleteBuger()
    {
        // 햄버거 프리팹이 할당되어 있는가
        if (hamburgerPrefab == null)
        {
            Debug.LogWarning("햄버거 프리팹이 할당되지 않았습니다!");
            return;
        }

        // 햄버거 생성 위치
        Vector3 bugerPosition = transform.position + Vector3.up * 0.3f; // <- 조리대 위

        // 햄버거 프리팹 생성
        GameObject burger = Instantiate(hamburgerPrefab, bugerPosition, Quaternion.identity);

        // 햄버거 이름 설정 (생성될 햄버거 프리팹의 이름을 설정하는 용도)
        burger.name = "완성된 햄버거";

        // 햄버거 완성 시 콘솔창에 안내
        Debug.Log("햄버거가 완성되었습니다!");

        // 주문 완료 상태 확인
        CheckOrderStatus();

        // 기존 재료 모두 제거
        ClearBurgerIngredients();
    }

    // 주문 완료 상태 확인
    private void CheckOrderStatus()
    {
        bool hasFrenchFries = sideMenus.ContainsKey(IngredientStation.IngredientType.FrenchFries);
        bool hasCola = sideMenus.ContainsKey(IngredientStation.IngredientType.Cola);

        string orderStatus = "주문 상태 : 햄버거(완성)";

        if (hasFrenchFries)
        {
            orderStatus += ", 감자튀김(준비됨)";
        }
        else
        {
            orderStatus += ", 콜라(준비 안 됨)";
        }
        
        if (hasCola)
        {
            orderStatus += ", 콜라(준비됨)";
        }
        else
        {
            orderStatus += ", 콜라(준비 안 됨)";
        }

        Debug.Log(orderStatus);
    }

    private void StackIngredients()
    {
        Vector3 basePosition = transform.position;
        float currentHeight = 0f;
        for (int i = 0; i < placedIngredients.Count; i++)
        {
            GameObject ingredient = placedIngredients[i];
            Vector3 newPosition = new Vector3(
                basePosition.x,
                basePosition.y + currentHeight,
                basePosition.z
            );
            ingredient.transform.position = newPosition;
            currentHeight += stackHeight;
        }
    }

    // 햄버거 재료만 제거 (사이드 메뉴 유지)
    public void ClearBurgerIngredients()
    {
        foreach (GameObject ingredient in placedIngredients)
        {
            Destroy(ingredient);
        }

        placedIngredients.Clear();
    }

    // 조리대에 있는 모든 음식 제거 (완성된 햄버거, 사이드 메뉴)
    public void ClearAllFoods()
    {
        // 햄버거 제거
        Destroy(hamburgerPrefab);

        // 사이드 메뉴 제거
        foreach(var sideMenu in sideMenus.Values)
        {
            Destroy(sideMenu);
        }
        sideMenus.Clear();
    }
}
