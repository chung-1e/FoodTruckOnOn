using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    [Header("UI ���")]
    public Text recipeDisplay; // �����Ǹ� ǥ���� Text ������Ʈ

    [Header("�ֻ��� į����Ʈ")]
    public RollDice diceRoller; // �ֻ��� ������ ������Ʈ

    [Header("��Ÿ ������Ʈ")]
    public CookingStation cookingStation;   //������ ����

    //���� ������
    private List<string> currentRecipe = new List<string>();
    private int requiredMainIngredients = 0;    // ���� ����� ����
    private bool requiresFrenchFries = false;
    private bool requiresCola = false;

    // ��� Ÿ�԰� �̸� ����
    private Dictionary<IngredientStation.IngredientType, string> ingredientNames;

    void Start()
    {
        InitializeIngredientNames();

        // �ֻ��� �ý��۰� ����
        if (diceRoller == null )
        {
            Debug.LogError("RollDice ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }

    void InitializeIngredientNames()
    {
        ingredientNames = new Dictionary<IngredientStation.IngredientType, string>
        {
            { IngredientStation.IngredientType.Bread, "��" },
            { IngredientStation.IngredientType.Patty, "��Ƽ" },
            { IngredientStation.IngredientType.Lettuce, "�����" },
            { IngredientStation.IngredientType.Tomato, "�丶��" },
            { IngredientStation.IngredientType.Cheese, "ġ��" },
            { IngredientStation.IngredientType.Shrimp, "����" },    // ���� �߰�
            { IngredientStation.IngredientType.Chicken, "ġŲ" },   // ġŲ �߰�
            { IngredientStation.IngredientType.FrenchFries, "����Ƣ��" },
            { IngredientStation.IngredientType.Cola, "�ݶ�" }
        };
    }


    public void GenerateRecipe(int diceNumber)
    {
        currentRecipe.Clear();

        // �Ʒ� ��
        currentRecipe.Add("�Ʒ� ��");

        // ���� ���� (�ֻ��� �� ����ŭ)
        requiredMainIngredients = diceNumber;
        for (int i = 0; i < diceNumber; i++)
        {
            // �������� ���� ��� ���� (��Ƽ, �����, �丶��, ġ��, ����, ġŲ �߿���)
            IngredientStation.IngredientType[] mainIngredients = {
            IngredientStation.IngredientType.Patty,
            IngredientStation.IngredientType.Lettuce,
            IngredientStation.IngredientType.Tomato,
            IngredientStation.IngredientType.Cheese,
            IngredientStation.IngredientType.Shrimp,   // ���� �߰�
            IngredientStation.IngredientType.Chicken   // ġŲ �߰�
    };

            int randomIndex = Random.Range(0, mainIngredients.Length);
            string ingredientName = ingredientNames[mainIngredients[randomIndex]];
            currentRecipe.Add(ingredientName);
        }

        // �� ��
        currentRecipe.Add("�� ��");

        // ���̵� �޴� - ���� ��Ģ ���� (�ֻ��� 5: ����Ƣ��, �ֻ��� 6: ����Ƣ��+�ݶ�)
        requiresFrenchFries = (diceNumber >= 5);
        requiresCola = (diceNumber == 6);

        if (requiresFrenchFries)
        {
            currentRecipe.Add("����Ƣ��");
        }

        if (requiresCola)
        {
            currentRecipe.Add("�ݶ�");
        }

        // ������ ǥ��
        DisplayRecipe();
    }

    void DisplayRecipe()
    {
        if (recipeDisplay ==  null)
        {
            Debug.LogError("������ ǥ�� Text ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        string recipeText = "[ ������ ]\n\n";

        foreach (string ingredient in currentRecipe)
        {
            recipeText += ingredient + "\n";
        }

        recipeDisplay.text = recipeText;
    }

    public void CheckRecipeComplettion()
    {
        // ���� �����뿡 �ִ� ��� ��� ��������
        List<string> placedItems = GetPlacedItemsInOrder();

        //�����ǿ� ��
        if (IsRecipeCorrect(placedItems))
        {
            Debug.Log("�Ϻ��մϴ�!");
            // ���⿡ ���� ó�� �ڵ� �߰� ����
        }
        else
        {
            Debug.Log("�߸� ��������ϴ�.");
            cookingStation.ClearAllFoods();
        }
    }

    List<string> GetPlacedItemsInOrder()
    {
        List<string> placedItems = new List<string>();

        //CookingStation���� ��� ���� ��������
        foreach (GameObject ingredient in cookingStation.placedIngredients)
        {
            if (ingredient != null)
            {
                string name = ingredient.name;

                // "�Ʒ� ��"�� "�� ��"�� ��Ȯ�� ����
                if (name.Contains("�Ʒ� ��") || name == "bottom_Bread")
                {
                    placedItems.Add("�Ʒ� ��");
                }
                else if (name.Contains("�� ��") || name == "top_Bread")
                {
                    // ��Ÿ ����
                    Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
                    if (ingredientComponent != null && ingredientNames.ContainsKey(ingredientComponent.type))
                    {
                        placedItems.Add(ingredientNames[ingredientComponent.type]);
                    }
                }
            }
        }

        // ���̵� �޴� Ȯ��
        if (cookingStation.sideMenus.ContainsKey(IngredientStation.IngredientType.FrenchFries))
        {
            placedItems.Add("����Ƣ��");
        }

        if (cookingStation.sideMenus.ContainsKey(IngredientStation.IngredientType.Cola))
        {
            placedItems.Add("�ݶ�");
        }

        return placedItems;
    }

    bool IsRecipeCorrect(List<string> placedItems)
    {
        // ���̰� �ٸ��� ����
        if (placedItems.Count != currentRecipe.Count)
        {
            return false;
        }

        // �� �׸��� ������� ��Ȯ���� Ȯ��
        for (int i = 0; i < currentRecipe.Count; i++)
        {
            if (placedItems[i] != currentRecipe[i])
            {
                return false;
            }
        }

        return true;
    }
}
            
        
    


