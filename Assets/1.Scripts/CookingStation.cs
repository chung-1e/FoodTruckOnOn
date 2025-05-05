using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    //RecipeManager���� ���� �����ϵ���
    // �ܹ��� ����
    public List<GameObject> placedIngredients = new List<GameObject>();

    public Dictionary<IngredientStation.IngredientType, GameObject> sideMenus
        = new Dictionary<IngredientStation.IngredientType, GameObject>();

    public float stackHeight = 0.2f;

    public GameObject hamburgerPrefab;

    // ���̵� �޴��� ���� ��ġ
    public Transform rightSidePosition;
    public Transform leftSidePosition;

    [Header("������ �ý���")]
    public RecipeManager recipeManager; // ������ �Ŵ��� ����

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

        // ���̵� �޴����� Ȯ��
        if (ingredientComponent.isSideMenu)
        {
            AddSideMenu(ingredient, ingredientComponent.type);
        }
        else
        {
            // �ܹ��� ��� �߰�
            placedIngredients.Add(ingredient);
            Debug.Log($"��� �߰�: {ingredient.name}, ���� ��� ����: {placedIngredients.Count}");

            ingredient.transform.SetParent(transform);

            StackIngredients();

            // ���� Ȯ�� (���̵� �޴��� �ƴ� ��츸)
            if (recipeManager != null)
            {
                recipeManager.CheckIngredientOrder();
            }
        }
    }

    private void AddSideMenu(GameObject sideMenu, IngredientStation.IngredientType type)
    {
        // �̹� ���� ������ ���̵� �޴��� �ִٸ� ����
        if (sideMenus.ContainsKey(type))
        {
            Destroy(sideMenus[type]);
            sideMenus.Remove(type);
        }

        // ���̵� �޴� ��ġ ��ġ ����
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
            // �� �� ���� ���̵� �޴��� ���� �� ����
            Debug.LogWarning("�� �� ���� ���̵� �޴� ���� : " + type);
            Destroy(sideMenu);
            return;
        }

        // ���̵� �޴� ��ġ ����
        if (targetPosition != null)
        {
            sideMenu.transform.position = targetPosition.position;
            sideMenu.transform.SetParent(targetPosition);

            // ���̵� �޴� ���
            sideMenus[type] = sideMenu;

            Debug.Log(type + "���̵� �޴��� ������ ���� ��ġ�Ǿ����ϴ�.");

            // ���̵� �޴��� �߰��Ǿ��� ���� ������ üũ
            if (recipeManager != null)
            {
                recipeManager.CheckIngredientOrder();
            }
        }
        else
        {
            Debug.Log("���̵� �޴� ��ġ ��ġ�� �������� �ʾҽ��ϴ�.");
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

    // �ܹ��� ��Ḹ ���� (���̵� �޴� ����)
    public void ClearBurgerIngredients()
    {
        foreach (GameObject ingredient in placedIngredients)
        {
            Destroy(ingredient);
        }

        placedIngredients.Clear();
    }

    // �����뿡 �ִ� ��� ���� ���� (�ϼ��� �ܹ���, ���̵� �޴�)
    public void ClearAllFoods()
    {
        ClearBurgerIngredients();

        // ���̵� �޴� ����
        foreach (var sideMenu in sideMenus.Values)
        {
            Destroy(sideMenu);
        }
        sideMenus.Clear();
    }
}
