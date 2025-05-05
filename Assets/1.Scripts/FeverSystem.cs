using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverSystem : MonoBehaviour
{
    [Header("UI 요소")]
    public Image frameImage;    // 큰 툴 이미지
    public Image[] streakImages;  // 5개의 이미지 배열 (0이 아래, 4가 위)
    public GameObject feverPanel;   // 피버 타임 패널

    [Header("이미지 스트라이트")]
    public Sprite[] grayscaleSprites;  // 서로 다른 5개의 흑백 이미지
    public Sprite[] colorSprites;  // 서로 다른 5개의 컬러 이미지

    [Header("피버 타임 설정")]
    public float feverDuration = 10f;   // 피버 타임 지속시간

    private int currentStreak = 0;  // 현재 연속 성공 횟수
    private bool isInFever = false; // 피버 타임 중인지

    void Start()
    {
        // 초기화 - 모든 이미지를 흑백으로 설정
        ResetStreak();

        // 피버 패널 비활성화
        if (feverPanel != null )
        {
            feverPanel.SetActive(false);
        }
    }

    // 햄버거 완성 시 호출
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

    // 실패 시 호출
    public void OnBurgerFailed()
    {
        if (isInFever)
        {
            ResetStreak();
        }
    }

    // 연속 성공 디스플레이 업데이트
    private void UpdateStreakDiplay()
    {
        for (int i = 0; i < streakImages.Length; i++)
        {
            if (i < currentStreak)
            {
                // 성공한 만큼 컬러 이미지로 변경
                streakImages[i].sprite = colorSprites[i];
            }
            else
            {
                // 아직 달성하지 못한 것도 흑백으로 유지
                streakImages[i].sprite = grayscaleSprites[i];
            }
        }
    }

    // 피버 타임 시작
    private void StartFever()
    {
        isInFever = true;
        feverPanel.SetActive(true);
        StartCoroutine(FeverTimer());
    }

    // 피버 타임 타이머
    IEnumerator FeverTimer()
    {
        yield return new WaitForSeconds(feverDuration);
        EndFever();
    }

    // 피버 타임 종료
    private void EndFever()
    {
        isInFever = false;
        feverPanel.SetActive(false);
        ResetStreak();
    }

    // 연속 성공 리셋
    private void ResetStreak()
    {
        currentStreak = 0;
        UpdateStreakDiplay();
    }

    // 현재 피버 타임인지 확인
    public bool IsInFever()
    {
        return isInFever;
    }

}
