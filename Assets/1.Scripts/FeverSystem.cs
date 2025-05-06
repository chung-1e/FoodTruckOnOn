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
    public float autoBuildCooldown = 0.1f; // ��Ÿ ���� ��ٿ� �ð� (ª�� ����)

    [Header("�ʿ� ������Ʈ �Ҵ�")]
    public CookingStation cookingStation;
    public RecipeManager recipeManager;
    public RollDice diceRoller;

    private int currentStreak = 0;  // ���� ���� ���� Ƚ��
    private bool isInFever = false; // �ǹ� Ÿ�� ������
    private bool isProcessingHamburger = false; // �ܹ��� ó�� �� ���� �߰�
    private float lastBuildTime = 0f; // ���������� �ܹ��Ÿ� ���� �ð�

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

        Debug.Log("FeverSystem ���۵�: streakImages ���� = " + streakImages.Length);
    }

    void Update()
    {
        // ��, �ܹ��� ó�� ���� ���� ����
        // �ǹ� Ÿ�� ���̰� �����̽��ٸ� ���� ���, �ڵ����� ��� �ױ�
        if (isInFever && Input.GetKeyDown(KeyCode.Space))
        {
            // ��ٿ� �ð� Ȯ�� (������ ���� �� ���� �ð��� ��������)
            float currentTime = Time.time;
            if (currentTime - lastBuildTime >= autoBuildCooldown && !isProcessingHamburger)
            {
                if (cookingStation != null && recipeManager != null)
                {
                    Debug.Log("�ǹ� Ÿ�� �� �����̽��� ���� - �ڵ� �ܹ��� ����");
                    AutoBuildBurger();
                    lastBuildTime = currentTime; // ������ ���� �ð� ������Ʈ
                }
            }
            else
            {
                Debug.Log($"��ٿ� �� ({autoBuildCooldown - (currentTime - lastBuildTime)}�� ����)");
            }
        }
    }

    // �ܹ��Ÿ� �ڵ����� ����� �Լ�
    public void AutoBuildBurger()
    {
        if (!isInFever || cookingStation == null || recipeManager == null) return;

        // �̹� ó�� ���̸� �ߺ� ���� ����
        if (isProcessingHamburger)
        {
            Debug.Log("�̹� �ܹ��� ó�� �� - ��û ����");
            return;
        }

        // �ܹ��� ó�� ���� ���·� ��ȯ
        isProcessingHamburger = true;
        Debug.Log("�ܹ��� �ڵ� ���� ���� - ó�� �� ���·� ��ȯ");

        // �߿�: ���� �����븦 �ʱ�ȭ�ؾ� ��
        cookingStation.ClearAllFoods();

        // �����ǿ� ���� ��Ḧ �ڵ����� �ױ�
        List<string> recipe = recipeManager.GetCurrentRecipe();
        foreach (string ingredientName in recipe)
        {
            GameObject newIngredient = CreateIngredientByName(ingredientName);
            if (newIngredient != null)
            {
                cookingStation.AddIngredient(newIngredient);
            }
        }

        // ������ �ϼ� Ȯ�� - ��Ʈ�� ���� ���� Ȯ�θ� ����
        StartCoroutine(CompleteRecipeAfterDelay(0.01f)); // �ణ�� ������ �� �ϼ� ó��
    }

    // �ణ�� ������ �� ������ �ϼ� ó�� (��� ��ᰡ ���̵���)
    IEnumerator CompleteRecipeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        recipeManager.CheckRecipeCompletion();

        // ���� �ֻ����� ������ �� ó�� ���� ����
        StartCoroutine(ResetProcessingState());
    }

    // �ܹ��� ó�� ���¸� �ʱ�ȭ�ϴ� �ڷ�ƾ
    IEnumerator ResetProcessingState()
    {
        // ª�� ������ �� ó�� ���� ���� (���� �ֻ����� ������ ��)
        yield return new WaitForSeconds(0.3f);
        isProcessingHamburger = false;
        Debug.Log("�ܹ��� ó�� ���� �ʱ�ȭ �Ϸ� - ���� �Է� ����");
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
        if (isInFever)
        {
            Debug.Log("�ǹ� Ÿ�� �� ������ ��Ʈ���� ���� ����");
            return;
        }

        currentStreak++;
        Debug.Log($"�ܹ��� ����! ���� ��Ʈ��: {currentStreak}/{streakImages.Length}");
        UpdateStreakDisplay();

        if (currentStreak >= streakImages.Length)
        {
            StartFever();
        }
    }

    // ���� �� ȣ��
    public void OnBurgerFailed()
    {
        if (!isInFever)
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
    private void UpdateStreakDisplay()
    {
        if (streakImages == null || streakImages.Length == 0)
        {
            Debug.LogError("��Ʈ�� �̹��� �迭�� ���ų� ����ֽ��ϴ�!");
            return;
        }

        if (grayscaleSprites == null || grayscaleSprites.Length == 0 ||
            colorSprites == null || colorSprites.Length == 0)
        {
            Debug.LogError("��������Ʈ �迭�� ���ų� ����ֽ��ϴ�!");
            return;
        }

        Debug.Log($"��Ʈ�� ���÷��� ������Ʈ - ���� ��Ʈ��: {currentStreak}");

        for (int i = 0; i < streakImages.Length; i++)
        {
            if (streakImages[i] == null)
            {
                Debug.LogError($"��Ʈ�� �̹��� {i}���� null�Դϴ�!");
                continue;
            }

            if (i < currentStreak)
            {
                if (i < colorSprites.Length)
                {
                    streakImages[i].sprite = colorSprites[i];
                    Debug.Log($"�̹��� {i}: �÷� �̹����� ����");
                }
            }
            else
            {
                if (i < grayscaleSprites.Length)
                {
                    streakImages[i].sprite = grayscaleSprites[i];
                    Debug.Log($"�̹��� {i}: ��� �̹����� ����");
                }
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
        Debug.Log($"��Ʈ�� �ʱ�ȭ: {currentStreak} -> 0");
        currentStreak = 0;
        UpdateStreakDisplay();
    }

    // ���� �ǹ� Ÿ������ Ȯ��
    public bool IsInFever()
    {
        return isInFever;
    }

}
