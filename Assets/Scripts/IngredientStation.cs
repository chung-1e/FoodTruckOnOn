using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientStation : MonoBehaviour
{
    public enum IngredientType
    {
        Bread,              // ��
        Patty,               // ��Ƽ
        Lettuce,           // �����
        FrenchFries,     // ����Ƣ��
        Tomato,           // �丶��
        Cheese,            // ġ��
        Cola                 // �ݶ�
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
            Debug.LogError(GetIngredientName() + "�����̼ǿ� �������� �Ҵ���� �ʾҽ��ϴ�!");
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
                return "��";
            case IngredientType.Patty:
                return "��Ƽ";
            case IngredientType.Lettuce:
                return "�����";
            case IngredientType.FrenchFries:
                return "����Ƣ��";
            case IngredientType.Tomato:
                return "�丶��";
            case IngredientType.Cheese:
                return "ġ��";
            case IngredientType.Cola:
                return "�ݶ�";
            default:
                return "�� �� ����";
        }
    }

    private GameObject SpawnBread()
    {
        CookingStation cookingStation = FindObjectOfType<CookingStation>();

        if (cookingStation == null)     // �����뽺ũ��Ʈ�� ȭ�鿡 ������ ���
        {
            Debug.LogError("�����븦 ã�� �� �����ϴ�!");
            return null;
        }

        bool hasIngredientsOnStation = cookingStation.HasAnyIngredients();
        GameObject prefabToUse;
        string breadName;

        if (hasIngredientsOnStation)
        {
            prefabToUse = breadTopPrefab;
            breadName = "�� ��";
        }
        else
        {
            prefabToUse = breadBottomPrefab;
            breadName = "�Ʒ� ��";
        }

        if (prefabToUse == null)
        {
            Debug.LogError(breadName + "�������� �Ҵ���� �ʾҽ��ϴ�!");
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
