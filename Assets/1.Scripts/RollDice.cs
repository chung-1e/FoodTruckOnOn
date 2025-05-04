using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollDice : MonoBehaviour
{
    [Header("�ֻ��� �̹��� ����")]
    public Image diceImage; // �ֻ��� �̹����� ǥ���Ҷ� Image ������Ʈ
    public Sprite[] diceSprites;    // 1~6������ �ֻ��� ��������Ʈ �迭

    [Header("�Ѹ� ����")]
    public Button rollButton;   // �ֻ����� ���� ��ư
    public float rollingDuration = 0.5f;    // �������� �ð�
    public float swapInterval = 0.5f;   // �̹��� ���� ����

    [Header("������ �ý���")]
    public RecipeManager recipeManager; // ������ �Ŵ��� ����

    [Header("������ �ý���")]
    public Vector2 dicePos; //�ֻ����� �̵��Ҷ� ��ǥ ��ġ
    public float shrinkDuration = 0.5f; //ũ�� �ӵ� �� �̵� �ð�

    private bool isRolling = false; //���� �ֻ����� �������� �ִ��� Ȯ��
    private RectTransform diceRectTransform;    // �ֻ����� RectTransform

    private Vector2 originalSize = new Vector2(450, 450); // ���� ũ��
    private Vector2 reducedSize = new Vector2(112.5f, 112.5f);  // ��ҵ� ũ��

    void Start()
    {
        // ������Ʈ ����
        diceRectTransform = diceImage.GetComponent<RectTransform>();

        // ��ư Ŭ�� �̺�Ʈ ����
        if (rollButton != null)
        {
            rollButton.onClick.AddListener(Rolling);
        }

        // ���� �� �ֻ��� �ʱ�ȭ
        if (diceImage != null && diceSprites.Length > 0)
        {
            diceImage.sprite = diceSprites[0];
            // �ʱ� ��ġ�� ũ�� ����
            diceRectTransform.anchoredPosition = Vector2.zero;
            diceRectTransform.sizeDelta = reducedSize;
        }
    }

    public void Rolling()
    {
        // �̹� �������� �ִٸ� ����
        if (isRolling) return;

        // �ڷ�ƾ ����
        StartCoroutine(RollDiceCoroutine());
    }


    IEnumerator RollDiceCoroutine()
    {
        isRolling = true;

        // ��ư ��Ȱ��ȭ (���� Ŭ�� ����)
        if (rollButton != null)
        {
            rollButton.interactable = false;
        }

        // �ֻ����� (0,0) ��ġ�� �̵��ϰ� 450x450���� ũ�� ����
        diceRectTransform.anchoredPosition = Vector2.zero;
        diceRectTransform.sizeDelta = reducedSize;

        // �������� ȿ��
        float elepsedTime = 0f;
        while (elepsedTime < rollingDuration)
        {
            //�����ϰ� �ֻ��� �̹��� ����
            int randomIndex = Random.Range(0, diceSprites.Length);
            diceImage.sprite = diceSprites[randomIndex];

            elepsedTime += swapInterval;
            yield return new WaitForSeconds(swapInterval);
        }

        // ���� ���� ��� ǥ��
        int finalResult = Random.Range(0, diceSprites.Length);
        diceImage.sprite = diceSprites[finalResult];

        // �ֻ��� ��� (1~6)
        int diceNumber = finalResult + 1;
        Debug.Log($"�ֻ��� ���: {diceNumber}");

        // ������ ����
        if (recipeManager != null)
        {
            recipeManager.GenerateRecipe(diceNumber);
        }

        //ũ�⸦ ���̸鼭 dicePos�� �̵��ϴ� �ڷ�ƾ ����
        yield return StartCoroutine(ShrinkAndMoveToDicePos());

        //��ư �ٽ� Ȱ��ȭ
        if (rollButton !=null)
        {
            rollButton.interactable = true;
        }

        isRolling = false;
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

            // ��ġ�� ũ�⸦ ���������� ����
            diceRectTransform.anchoredPosition = Vector2.Lerp(startPosition, dicePos, smoothT);
            diceRectTransform.sizeDelta = Vector2.Lerp(startSize, reducedSize, smoothT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���� ��ġ�� ũ�� ���� (����)
        diceRectTransform.anchoredPosition = dicePos;
        diceRectTransform.sizeDelta = reducedSize;
    }
}
