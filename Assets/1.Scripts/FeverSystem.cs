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
    public float feverDuration = 10f;   // �ǹ� Ÿ�� ���ӽð�

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
            ResetStreak();
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
        feverPanel.SetActive(true);
        StartCoroutine(FeverTimer());
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
        feverPanel.SetActive(false);
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
