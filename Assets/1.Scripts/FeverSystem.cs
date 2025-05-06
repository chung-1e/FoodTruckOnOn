using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverSystem : MonoBehaviour
{
    [Header("UI ���")]
    public Image frameImage;    // ū �� �̹���
    public Image[] streakImages;  // 5���� �̹��� �迭 (0�� �Ʒ�, 4�� ��)
    public GameObject feverPanel;   // �ǹ� Ÿ�� �г�

    [Header("�̹��� ��Ʈ����Ʈ")]
    public Sprite[] grayscaleSprites;  // ���� �ٸ� 5���� ��� �̹���
    public Sprite[] colorSprites;  // ���� �ٸ� 5���� �÷� �̹���

    [Header("�ǹ� Ÿ�� ����")]
    public float feverDuration = 10f;   // �ǹ� Ÿ�� ���ӽð�: 10��
    public float feverPanelDuration = 1f; // �ǹ� �г� ǥ�� �ð�: 1��

    // �߰�
    [Header("�ʿ� ������Ʈ �Ҵ�")]
    public CookingStation cookingStation;
    public RecipeManager recipeManager;
    public RollDice diceRoller;

    private int currentStreak = 0;  // ���� ���� ���� Ƚ��
    private bool isInFever = false; // �ǹ� Ÿ�� ������

    void Start()
    {
        // �ʱ�ȭ - ��� �̹����� ������� ����
        ResetStreak();

        // �ǹ� �г� ��Ȱ��ȭ
        if (feverPanel != null )
        {
            feverPanel.SetActive(false);
        }

        // �ʿ��� ������Ʈ���� ã�Ƽ� �Ҵ�
        if (cookingStation == null)
            cookingStation = FindObjectOfType<CookingStation>();
        if (recipeManager == null)
            recipeManager = FindObjectOfType<RecipeManager>();
        if (diceRoller == null)
            diceRoller = FindObjectOfType<RollDice>();
    }

    private void Update()
    {
        // �ǹ� Ÿ�� ���̰� �����̽��ٸ� ���� ���, �ڵ����� ��� �ױ�
        if (isInFever && Input.GetKeyDown(KeyCode.Space))
        {
            if(cookingStation != null && recipeManager != null)
            {
                AutoBuildBurger();
            }
        }
    }

    // �ܹ��Ÿ� �ڵ����� ����� ���
    public void AutoBuildBurger()
    {
        if (!isInFever || cookingStation == null || recipeManager == null) return;

        // ���� ������ �ʱ�ȭ
        cookingStation.ClearAllFoods();

        // �����Ƿ� ���� ���� �ڵ����� �ױ�
        List<string> recipe = recipeManager.GetCurrentRecipe();
        foreach (string ingredientName in recipe)
        {
            GameObject newIngredient = CreateIngredientByName(ingredientName);
            if (newIngredient != null )
            {
                cookingStation.AddIngredient(newIngredient);
            }
        }

        // ������ �ϼ� ȭ�� - �޺� ������ ���� ���� Ȯ�θ� ����
        recipeManager.CheckRecipeCompletion();
    }

    // ��� �̸��� ���� �� ��Ḧ �����ϴ� �Լ�
    private GameObject CreateIngredientByName(string ingredientName)
    {
        IngredientStation[] stations = FindObjectsOfType<IngredientStation>();
        foreach (IngredientStation station in stations)
        {
            if (station.GetIngredientName() == ingredientName ||
                (ingredientName == "�Ʒ� ��" && station.stationType == IngredientStation.IngredientType.Bread) ||
                (ingredientName == "�� ��" && station.stationType == IngredientStation.IngredientType.Bread))
            {
                return station.SpawnIngredient();
            }
        }
        return null;
    }

    // �ܹ��� �ϼ� �� ȣ��
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

    // ���� �� ȣ��
    public void OnBurgerFailed()
    {
        if (isInFever)
        {
            Debug.Log("�ܹ��� ����! �޺� ������ �ʱ�ȭ");
            ResetStreak();
        }
        else
        {
            // Ȥ�ö� ���װ� �߻��� ���� ����Ͽ�
            Debug.Log("�ǹ� Ÿ�� �� ���д� �޺� ������ ���� ����");
        }
    }

    // ���� ���� ���÷��� ������Ʈ
    private void UpdateStreakDiplay()
    {
        for (int i = 0; i < streakImages.Length; i++)
        {
            if (i < currentStreak)
            {
                // ������ ��ŭ �÷� �̹����� ����
                streakImages[i].sprite = colorSprites[i];
            }
            else
            {
                // ���� �޼����� ���� �͵� ������� ����
                streakImages[i].sprite = grayscaleSprites[i];
            }
        }
    }

    // �ǹ� Ÿ�� ����
    private void StartFever()
    {
        isInFever = true;
        Debug.Log("�ǹ� Ÿ�� ����!");

        if (feverPanel != null)
        {
            feverPanel.SetActive(true);
            // �г��� 1�ʸ� ǥ���ϰ� ��������, �ǹ� ���´� ��� ����
            StartCoroutine(HideFeverPanel());
        }

        // �ǹ� Ÿ���� 10�� ���� ����
        StartCoroutine(FeverTimer());
    }

    IEnumerator HideFeverPanel()
    {
        yield return new WaitForSeconds(feverPanelDuration);
        if (feverPanel != null)
        {
            feverPanel.SetActive(false);
            Debug.Log(" �ǹ� �г� ���� (�ǹ� Ÿ���� ��� ����)");
        }
    }

    // �ǹ� Ÿ�� Ÿ�̸�
    IEnumerator FeverTimer()
    {
        yield return new WaitForSeconds(feverDuration);
        EndFever();
    }

    // �ǹ� Ÿ�� ����
    private void EndFever()
    {
        isInFever = false;
        if (feverPanel != null)
        {
            feverPanel.SetActive(false);
        }
        ResetStreak();
    }

    // ���� ���� ����
    private void ResetStreak()
    {
        currentStreak = 0;
        UpdateStreakDiplay();
    }

    // ���� �ǹ� Ÿ������ Ȯ��
    public bool IsInFever()
    {
        return isInFever;
    }

}
