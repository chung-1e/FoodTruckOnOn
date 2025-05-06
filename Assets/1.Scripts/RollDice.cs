using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RollDice : MonoBehaviour
{
    [Header("�ֻ��� �̹��� ����")]
    public Image diceImage;  // �ֻ��� �̹����� ǥ���� Image ������Ʈ
    public Sprite[] diceSprites;  // 1~6������ �ֻ��� ��������Ʈ �迭

    [Header("�Ѹ� ����")]
    public float rollingDuration = 0.5f;  // �������� �ð�
    public float swapInterval = 0.05f;  // �̹��� ���� ����

    [Header("������ �ý���")]
    public RecipeManager recipeManager;  // ������ �Ŵ��� ����

    [Header("�ִϸ��̼� ����")]
    public Vector2 dicePos;  // �ֻ����� �̵��� ���� ��ġ
    public float shrinkDuration = 0.5f;  // ũ�� ��� �� �̵� �ð�

    [Header("�ð� ����")]
    public GameObject blackPanel;  // �� �г� GameObject

    [Header("�ǹ� �ý���")]
    public FeverSystem feverSystem;

    private bool isRolling = false;  // ���� �ֻ����� �������� �ִ��� Ȯ��
    private RectTransform diceRectTransform;  // �ֻ����� RectTransform
    private Vector2 originalSize = new Vector2(450, 450);  // ū ũ��
    private Vector2 reducedSize = new Vector2(112.5f, 112.5f);  // ���� ũ��

    public int currentDiceValue = 0; // ���� �ֻ��� ��

    void Start()
    {
        // ������Ʈ ����
        diceRectTransform = diceImage.GetComponent<RectTransform>();

        // ���� �� �ֻ��� �ʱ�ȭ - (0,0) ��ġ�� ū ũ��� ��ġ
        if (diceImage != null && diceSprites.Length > 0)
        {
            diceImage.sprite = diceSprites[0];
            diceRectTransform.anchoredPosition = Vector2.zero;
            diceRectTransform.sizeDelta = originalSize;
        }

        // ���� �� �� �г� ��Ȱ��ȭ
        if (blackPanel != null)
        {
            blackPanel.SetActive(false);
        }

        // �ǹ� �ý��� ���� ã��
        if (feverSystem == null)
        {
            feverSystem = FindObjectOfType<FeverSystem>();
        }

        // ���� ���� �� ù ��° �ֻ��� ������
        Rolling();
    }

    public void Rolling()
    {
        // �̹� �������� �ִٸ� ����
        if (isRolling) return;

        // �ǹ� Ÿ�� ���̸� ���� �Ѹ� ����
        if (feverSystem != null && feverSystem.IsInFever())
        {
            StartCoroutine(QuickRollDiceCoroutine());
        }
        else
        {
            StartCoroutine(RollDiceCoroutine());
        }
    }

    IEnumerator RollDiceCoroutine()
    {
        isRolling = true;

        // �� �г� Ȱ��ȭ
        if (blackPanel != null)
        {
            blackPanel.SetActive(true);
        }

        // �ð� ���� (�ֻ��� ����)
        Time.timeScale = 0f;

        // �ֻ����� (0,0) ��ġ�� ū ũ��� ���� (���� ��ġ)
        diceRectTransform.anchoredPosition = Vector2.zero;
        diceRectTransform.sizeDelta = originalSize;

        // ������ ����
        if (recipeManager != null)
        {
            recipeManager.OnNewRecipeGenerated();
        }

        // �������� ȿ��
        float elapsedTime = 0f;
        while (elapsedTime < rollingDuration)
        {
            // �����ϰ� �ֻ��� �̹��� ����
            int randomIndex = Random.Range(0, diceSprites.Length);
            diceImage.sprite = diceSprites[randomIndex];
            elapsedTime += swapInterval;

            // ���� �ð� ��� (Time.timeScale = 0�̾ �۵�)
            yield return new WaitForSecondsRealtime(swapInterval);
        }

        // ���� ���� ��� ǥ��
        int finalResult = Random.Range(0, diceSprites.Length);
        diceImage.sprite = diceSprites[finalResult];

        // �ֻ��� ��� (1~6)
        int diceNumber = finalResult + 1;
        currentDiceValue = diceNumber; // ���� �� ����
        Debug.Log($"�ֻ��� ���: {diceNumber}");

        //������ ������ �� ���� ȣ���
        if (recipeManager != null)
        {
            recipeManager.GenerateRecipe(diceNumber);
        }

        // ũ�⸦ ���̸鼭 dicePos�� �̵��ϴ� �ڷ�ƾ ����
        yield return StartCoroutine(ShrinkAndMoveToDicePos());

        // �ֻ��� �̵��� ������ �ð� ����ȭ
        Time.timeScale = 1f;

        // �� �г� ��Ȱ��ȭ
        if (blackPanel != null)
        {
            blackPanel.SetActive(false);
        }

        isRolling = false;
    }

    // �ǹ� Ÿ�� �� ���� �ֻ��� ���� �ڷ�ƾ
    IEnumerator QuickRollDiceCoroutine()
    {
        isRolling = true;

        // ���� ������ �ʱ�ȭ
        if (recipeManager != null)
        {
            recipeManager.OnNewRecipeGenerated();
        }

        // ��� ��� ����
        int finalResult = Random.Range(0, diceSprites.Length);
        diceImage.sprite = diceSprites[finalResult];
        int diceNumber = finalResult + 1;
        currentDiceValue = diceNumber; // ���� �� ����
        Debug.Log($"[�ǹ�Ÿ��] �ֻ��� ���: {diceNumber}");

        // �ֻ����� �׻� ���� ũ��� ������ ��ġ�� ����
        diceRectTransform.anchoredPosition = dicePos;
        diceRectTransform.sizeDelta = reducedSize;

        if (recipeManager != null)
        {
            recipeManager.GenerateRecipe(diceNumber);
        }

        isRolling = false;
        yield break;
    }

    IEnumerator ShrinkAndMoveToDicePos()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = Vector2.zero;
        Vector2 startSize = originalSize;

        while (elapsedTime < shrinkDuration)
        {
            float t = elapsedTime / shrinkDuration;
            // EaseOut ȿ���� ���� ���� (�� �ڿ������� ������)
            float smoothT = 1 - Mathf.Pow(1 - t, 2);

            // ��ġ�� ũ�⸦ ���������� ���� (���� �ð� ���)
            diceRectTransform.anchoredPosition = Vector2.Lerp(startPosition, dicePos, smoothT);
            diceRectTransform.sizeDelta = Vector2.Lerp(startSize, reducedSize, smoothT);

            elapsedTime += Time.unscaledDeltaTime;  // �ð� ���� ���¿����� �۵�
            yield return null;
        }

        // ���� ��ġ�� ũ�� ���� (����)
        diceRectTransform.anchoredPosition = dicePos;
        diceRectTransform.sizeDelta = reducedSize;
    }
}
