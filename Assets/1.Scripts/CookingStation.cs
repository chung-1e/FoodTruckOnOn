using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    //RecipeManager에서 접근 가능하도록
    // 햄버거 재료들
    public List<GameObject> placedIngredients = new List<GameObject>();

    public Dictionary<IngredientStation.IngredientType, GameObject> sideMenus
        = new Dictionary<IngredientStation.IngredientType, GameObject>();

    public float stackHeight = 0.2f;

    public GameObject hamburgerPrefab;

    // 사이드 메뉴가 놓일 위치
    public Transform rightSidePosition;
    public Transform leftSidePosition;

    [Header("레시피 시스템")]
    public RecipeManager recipeManager; // 레시피 매니저 참조

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
            Debug.Log($"재료 추가: {ingredient.name}, 현재 재료 개수: {placedIngredients.Count}");

            ingredient.transform.SetParent(transform);

            StackIngredients();

            // 순서 확인 (사이드 메뉴가 아닌 경우만)
            if (recipeManager != null)
            {
                recipeManager.CheckIngredientOrder();
            }
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

            // 사이드 메뉴가 추가되었을 때도 레시피 체크
            if (recipeManager != null)
            {
                recipeManager.CheckIngredientOrder();
            }
        }
        else
        {
            Debug.Log("사이드 메뉴 배치 위치가 설정되지 않았습니다.");
            Destroy(sideMenu);
        }
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
        ClearBurgerIngredients();

        // 사이드 메뉴 제거
        foreach (var sideMenu in sideMenus.Values)
        {
            Destroy(sideMenu);
        }
        sideMenus.Clear();
    }
}
