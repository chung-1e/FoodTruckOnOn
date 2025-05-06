using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    [Header("UI ���")]
    public Text recipeDisplay; // �����Ǹ� ǥ���� Text ������Ʈ

    [Header("�ֻ���")]
    public RollDice diceRoller; // �ֻ��� ������ ������Ʈ

    [Header("������")]
    public CookingStation cookingStation;   // ������ ����

    [Header("�ϼ� �ܹ��� ����")]
    public float burgerDisplayDuration = 2f;    // �ϼ� �ܹ��� ǥ�� �ð�

    [Header("�ǹ� �ý���")]
    public FeverSystem feverSystem; // �ǹ� �ý��� ����

    // ���� ������
    private List<string> currentRecipe = new List<string>();

    private int requiredMainIngredients = 0;    // ���� ����� ����

    private bool requiresFrenchFries = false;
    private bool requiresCola = false;

    // ��� Ÿ�԰� �̸� ����
    private Dictionary<IngredientStation.IngredientType, string> ingredientNames;

    //�ܹ��� �ϼ� ����
    private bool isBurgerCompleted = false;

    private GameObject completedBurger = null;

    void Start()
    {
        InitializeIngredientNames();

        // �ֻ��� �ý��۰� ����
        if (diceRoller == null )
        {
            Debug.LogError("RollDice ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
        }

        // �ǹ� �ý��� ���� ã��
        if (feverSystem == null)
        {
            feverSystem = FindObjectOfType<FeverSystem>();
            if (feverSystem == null)
            {
                Debug.LogError("�ǹ� �ý����� ã�� �� �����ϴ�!");
            }
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
            { IngredientStation.IngredientType.Shrimp, "����" },
            { IngredientStation.IngredientType.Chicken, "ġŲ" },
            { IngredientStation.IngredientType.FrenchFries, "����Ƣ��" },
            { IngredientStation.IngredientType.Cola, "�ݶ�" }
        };
    }

    // ���� ������ ��ȯ�ϴ� �Լ�
    public List<string> GetCurrentRecipe()
    {
        return currentRecipe;
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
            IngredientStation.IngredientType.Shrimp,
            IngredientStation.IngredientType.Chicken
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

    public void CheckIngredientOrder()
    {
        List<string> currentIngredients = GetCurrentIngredientsInOrder();

        Debug.Log("���� �ø� ���: " + string.Join(", ", currentIngredients));
        Debug.Log("������ ����: " + string.Join(", ", currentRecipe));
        Debug.Log($"���� ����: {currentIngredients.Count}, ������ ����: {currentRecipe.Count}");

        // ������� �ø� ��ᰡ ������ ������ �´��� Ȯ��
        for (int i = 0; i < currentIngredients.Count; i++)
        {
            // ������ ������ ����ų� ������ Ʋ�� ���
            if (i >= currentRecipe.Count || currentIngredients[i] != currentRecipe[i])
            {
                Debug.Log($"�߸��� ����! ��ġ {i}: ���� '{(i < currentRecipe.Count ? currentRecipe[i] : "����")}' ���� '{currentIngredients[i]}'");

                // �ǹ� Ÿ���� �ƴ� ���� ������ ���� ó��
                bool isInFever = (feverSystem != null && feverSystem.IsInFever());
                if (!isInFever)
                {
                    Debug.Log("���з� ���� ��Ʈ�� �ʱ�ȭ ȣ��!");
                    if (feverSystem != null)
                    {
                        feverSystem.OnBurgerFailed();
                    }
                    else
                    {
                        Debug.LogError("�ǹ� �ý����� null�Դϴ�! ��Ʈ�� �ʱ�ȭ �Ұ�!");
                    }
                }

                cookingStation.ClearAllFoods();
                return;
            }
        }

        // ��� ��ᰡ �����Ǵ�� �ö󰬴��� Ȯ��
        if (currentIngredients.Count == currentRecipe.Count)
        {
            Debug.Log("��� ��ᰡ �ùٸ� ������ �ö󰬽��ϴ�!");
            CheckRecipeCompletion();
        }
    }

    public void CheckRecipeCompletion()
    {
        // ���� �����뿡 �ִ� ��� ��� ��������
        List<string> placedItems = GetPlacedItemsInOrder();

        // �����ǿ� ��
        if (IsRecipeCorrect(placedItems))
        {
            Debug.Log("�Ϻ��մϴ�!");

            bool isInFever = (feverSystem != null && feverSystem.IsInFever());
            if (feverSystem != null && !isInFever)
            {
                feverSystem.OnBurgerCompleted();
            }

            CreateCompletedBurger();
        }
        else
        {
            Debug.Log("�߸� ��������ϴ�.");

            // �ǹ� Ÿ���� �ƴ� ���� ������ ���� ó��
            bool isInFever = (feverSystem != null && feverSystem.IsInFever());
            if (!isInFever)
            {
                Debug.Log("�߸��� �����Ƿ� ���� ��Ʈ�� �ʱ�ȭ ȣ��!");
                if (feverSystem != null)
                {
                    feverSystem.OnBurgerFailed();
                }
                else
                {
                    Debug.LogError("�ǹ� �ý����� null�Դϴ�! ��Ʈ�� �ʱ�ȭ �Ұ�!");
                }
            }

            cookingStation.ClearAllFoods();
        }
    }

    List<string> GetPlacedItemsInOrder()
    {
        List<string> placedItems = new List<string>();

        // 1. �ܹ��� ���� (�Ʒ� ������ �� ������ �������)
        foreach (GameObject ingredient in cookingStation.placedIngredients)
        {
            if (ingredient != null)
            {
                string name = ingredient.name;

                if (name.Contains("�Ʒ� ��") || name == "bread_bottom")
                {
                    placedItems.Add("�Ʒ� ��");
                }
                else if (name.Contains("�� ��") || name == "bread_top")
                {
                    placedItems.Add("�� ��");
                }
                else
                {
                    Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
                    if (ingredientComponent != null && ingredientNames.ContainsKey(ingredientComponent.type))
                    {
                        placedItems.Add(ingredientNames[ingredientComponent.type]);
                    }
                }
            }
        }

        // 2. ���̵� �޴��� (������ ������ ������� ������ �߰�)
        if (cookingStation.sideMenus.ContainsKey(IngredientStation.IngredientType.FrenchFries))
        {
            placedItems.Add("����Ƣ��");
        }

        if (cookingStation.sideMenus.ContainsKey(IngredientStation.IngredientType.Cola))
        {
            placedItems.Add("�ݶ�");
        }

        // ������ �α�
        Debug.Log("���� ��ġ�� �����۵�: " + string.Join(", ", placedItems));
        Debug.Log("���� ������: " + string.Join(", ", currentRecipe));

        return placedItems;
    }

    bool IsRecipeCorrect(List<string> placedItems)
    {
        // ���̰� �ٸ��� ����
        if (placedItems.Count != currentRecipe.Count) return false;

        // �� �׸��� ������� ��Ȯ���� Ȯ��
        for (int i = 0; i < currentRecipe.Count; i++)
        {
            if (placedItems[i] != currentRecipe[i]) return true;
        }

        return true;
    }

    private void CreateCompletedBurger()
    {
        Debug.Log("CreateCompletedBuger ȣ���");

        // ������ �� ��� ��� ����
        cookingStation.ClearAllFoods();

        // �ǹ� Ÿ�� ������ Ȯ��
        bool isInFever = false;
        if (feverSystem != null)
        {
            isInFever = feverSystem.IsInFever();
            Debug.Log("���� �ǹ� ����: " + (isInFever ? "�ǹ� Ÿ�� ��" : "�Ϲ� ���"));
        }
        else
        {
            Debug.LogError("�ǹ� �ý����� null�Դϴ�!");
        }

        // �ǹ� Ÿ���� �ƴ� ���� �ܹ��� ����
        if (!isInFever)
        {
            Debug.Log("�Ϲ� ��� - �ϼ��� �ܹ��� ����");
            Vector3 burgerPosition = cookingStation.transform.position + Vector3.up * 0.3f;
            completedBurger = Instantiate(cookingStation.hamburgerPrefab, burgerPosition, Quaternion.identity);
            completedBurger.name = "�ϼ��� �ܹ���";
            isBurgerCompleted = true;
        }
        else
        {
            // �ǹ� Ÿ�� �߿��� �ܹ��Ÿ� �������� �ʰ� �ٷ� ���� �ܰ��
            Debug.Log("�ǹ� Ÿ�� �� - �ܹ��� ���� �ǳʶ�");
            isBurgerCompleted = true;
            completedBurger = null;
        }

        Debug.Log("TriggerNextDice �ڷ�ƾ ����");
        StartCoroutine(TriggerNextDice());
    }

    public void OnNewRecipeGenerated()
    {
        // ���ο� �����ǰ� ������ �� ���� �ܹ��� ����
        if (completedBurger != null)
        {
            Destroy(completedBurger);
            completedBurger = null;
            isBurgerCompleted = false;
            Debug.Log("���� �ܹ��Ű� ���ŵǾ����ϴ�.");
        }
    }

    private List<string> GetCurrentIngredientsInOrder()
    {
        List<string> currentIngredients = new List<string>();

        // ���� �����뿡 �ö� �ִ� �ܹ��� ��Ḹ ��������
        foreach (GameObject ingredient in cookingStation.placedIngredients)
        {
            if (ingredient != null)
            {
                string name = ingredient.name;

                if (name.Contains("�Ʒ� ��") || name == "bread_bottom")
                {
                    currentIngredients.Add("�Ʒ� ��");
                }
                else if (name.Contains("�� ��") || name == "bread_top")
                {
                    currentIngredients.Add("�� ��");
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

        // ���̵� �޴��� Ȯ��
        if (cookingStation.sideMenus.ContainsKey(IngredientStation.IngredientType.FrenchFries))
        {
            currentIngredients.Add("����Ƣ��");
        }

        if (cookingStation.sideMenus.ContainsKey(IngredientStation.IngredientType.Cola))
        {
            currentIngredients.Add("�ݶ�");
        }

        return currentIngredients;
    }

    // �ڵ����� ���� �ֻ��� ����
    IEnumerator TriggerNextDice()
    {
        Debug.Log($"TriggerNextDice ���� - ���� Time.timeScale: {Time.timeScale}");

        // �ǹ� Ÿ�� ������ Ȯ��
        bool isInFever = (feverSystem != null && feverSystem.IsInFever());

        // �ǹ� Ÿ���� �ƴ� ���� ���
        if (!isInFever)
        {
            yield return new WaitForSecondsRealtime(burgerDisplayDuration);
        }
        else
        {
            // �ǹ� Ÿ���� ���� ��� ���� �ֻ��� (��� �ð� ����)
            yield return null;
        }

        Debug.Log("TriggerNextDice - �ֻ��� ������ ����");

        if (diceRoller != null)
        {
            diceRoller.Rolling();
        }
        else
        {
            Debug.Log("diceRoller�� null�Դϴ�!");
        }

        // �ܹ��� ó�� �Ϸ� ǥ��
        isBurgerCompleted = false;
    }

    // �ǹ� Ÿ�� �� ��Ÿ ���� ������ ���� �߰� �Լ�
    public bool IsProcessingRecipe()
    {
        return isBurgerCompleted;
    }
}