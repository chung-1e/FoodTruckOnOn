using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    // �ܹ��� ����
    private List<GameObject> placedIngredients = new List<GameObject>();

    private Dictionary<IngredientStation.IngredientType, GameObject> sideMenus
        = new Dictionary<IngredientStation.IngredientType, GameObject>();

    public float stackHeight = 0.2f;

    public GameObject hamburgerPrefab;

    // ���̵� �޴��� ���� ��ġ
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

        // ���̵� �޴����� Ȯ��
        if (ingredientComponent.isSideMenu)
        {
            AddSideMenu(ingredient, ingredientComponent.type);
        }
        else
        {
            // �ܹ��� ��� �߰�
            placedIngredients.Add(ingredient);

            ingredient.transform.SetParent(transform);

            StackIngredients();

            // �ܹ��� �ϼ� ���� Ȯ��
            CheckBurgerCompletion();
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
        }
        else
        {
            Debug.Log("���̵� �޴� ��ġ ��ġ�� �������� �ʾҽ��ϴ�.");
            Destroy(sideMenu);
        }
    }

    private void CheckBurgerCompletion()    // �ܹ��� �ϼ� ���� �޼���
    {
        //�Ʒ� ���� �����뿡 ���� (�⺻��)
        bool hasBreadBottom = false;

        //�� ���� �����뿡 ���� (�⺻��)
        bool hasBreadTop = false;


        // ��� ��Ḧ �˻��Ͽ� �Ʒ� ���� �� ���� �ִ��� Ȯ��
        foreach (GameObject ingredient in placedIngredients)
        {
            Ingredient ingredientComponenet = ingredient.GetComponent<Ingredient>();

            if (ingredientComponenet != null)
            {
                // ��� �̸����� �� ���� Ȯ��
                if (ingredient.name.Contains("�Ʒ� ��") || ingredient.name == "bread_bottom")
                {
                    hasBreadBottom = true;
                }
                else if (ingredient.name.Contains("�� ��") || ingredient.name == "bread_top")
                {
                    hasBreadTop = true;
                }
            }
        }

        // ���� ���� ��� ������ �ܹ��� �ϼ�
        if (hasBreadBottom && hasBreadTop)
        {
            CompleteBuger();
        }
    }

    private void CompleteBuger()
    {
        // �ܹ��� �������� �Ҵ�Ǿ� �ִ°�
        if (hamburgerPrefab == null)
        {
            Debug.LogWarning("�ܹ��� �������� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        // �ܹ��� ���� ��ġ
        Vector3 bugerPosition = transform.position + Vector3.up * 0.3f; // <- ������ ��

        // �ܹ��� ������ ����
        GameObject burger = Instantiate(hamburgerPrefab, bugerPosition, Quaternion.identity);

        // �ܹ��� �̸� ���� (������ �ܹ��� �������� �̸��� �����ϴ� �뵵)
        burger.name = "�ϼ��� �ܹ���";

        // �ܹ��� �ϼ� �� �ܼ�â�� �ȳ�
        Debug.Log("�ܹ��Ű� �ϼ��Ǿ����ϴ�!");

        // �ֹ� �Ϸ� ���� Ȯ��
        CheckOrderStatus();

        // ���� ��� ��� ����
        ClearBurgerIngredients();
    }

    // �ֹ� �Ϸ� ���� Ȯ��
    private void CheckOrderStatus()
    {
        bool hasFrenchFries = sideMenus.ContainsKey(IngredientStation.IngredientType.FrenchFries);
        bool hasCola = sideMenus.ContainsKey(IngredientStation.IngredientType.Cola);

        string orderStatus = "�ֹ� ���� : �ܹ���(�ϼ�)";

        if (hasFrenchFries)
        {
            orderStatus += ", ����Ƣ��(�غ��)";
        }
        else
        {
            orderStatus += ", �ݶ�(�غ� �� ��)";
        }
        
        if (hasCola)
        {
            orderStatus += ", �ݶ�(�غ��)";
        }
        else
        {
            orderStatus += ", �ݶ�(�غ� �� ��)";
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
        // �ܹ��� ����
        Destroy(hamburgerPrefab);

        // ���̵� �޴� ����
        foreach(var sideMenu in sideMenus.Values)
        {
            Destroy(sideMenu);
        }
        sideMenus.Clear();
    }
}
