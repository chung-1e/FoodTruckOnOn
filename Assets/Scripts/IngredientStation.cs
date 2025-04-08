using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientStation : MonoBehaviour
{
    public enum IngredientType
    {
        Bread,              // 빵
        Patty,               // 패티
        Lettuce,           // 양상추
        FrenchFries,     // 감자튀김
        Tomato,           // 토마토
        Cheese,            // 치즈
        Cola                 // 콜라
    }

    public IngredientType stationType;

    public GameObject IngredientPrefab;

    public GameObject breadBottomPrefab;
    public GameObject breadTopPrefab;

    public GameObject SpawnIngredient()
    {
        if (stationType == IngredientType.Bread)
        {
            return SpawnBread();
        }
        if (IngredientPrefab == null)
        {
            Debug.LogError(GetIngredientName() + "스테이션에 프리팹이 할당되지 않았습니다!");
            return null;
        }

        Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;

        GameObject newIngredient = Instantiate(IngredientPrefab, spawnPosition, Quaternion.identity);

        newIngredient.name = GetIngredientName();

        Ingredient ingredient = newIngredient.AddComponent<Ingredient>();
        ingredient.type = stationType;

        return newIngredient;
    }

    public string GetIngredientName()
    {
        switch (stationType)
        {
            case IngredientType.Bread:
                return "빵";
            case IngredientType.Patty:
                return "패티";
            case IngredientType.Lettuce:
                return "양상추";
            case IngredientType.FrenchFries:
                return "감자튀김";
            case IngredientType.Tomato:
                return "토마토";
            case IngredientType.Cheese:
                return "치즈";
            case IngredientType.Cola:
                return "콜라";
            default:
                return "알 수 없음";
        }
    }

    private GameObject SpawnBread()
    {
        CookingStation cookingStation = FindObjectOfType<CookingStation>();

        if (cookingStation == null)     // 조리대스크립트가 화면에 없으면 경고
        {
            Debug.LogError("조리대를 찾을 수 없습니다!");
            return null;
        }

        bool hasIngredientsOnStation = cookingStation.HasAnyIngredients();
        GameObject prefabToUse;
        string breadName;

        if (hasIngredientsOnStation)
        {
            prefabToUse = breadTopPrefab;
            breadName = "위 빵";
        }
        else
        {
            prefabToUse = breadBottomPrefab;
            breadName = "아래 빵";
        }

        if (prefabToUse == null)
        {
            Debug.LogError(breadName + "프리팹이 할당되지 않았습니다!");
            return null;
        }

        Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
        GameObject newBread = Instantiate(prefabToUse, spawnPosition, Quaternion.identity);

        newBread.name = breadName;

        Ingredient ingredient = newBread.AddComponent<Ingredient>();
        ingredient.type = IngredientType.Bread;

        return newBread;
    }
}
