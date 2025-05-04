using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollDice : MonoBehaviour
{
    [Header("주사위 이미지 설정")]
    public Image diceImage; // 주사위 이미지를 표시할때 Image 컴포넌트
    public Sprite[] diceSprites;    // 1~6까지의 주사위 스프라이트 배열

    [Header("롤링 설정")]
    public Button rollButton;   // 주사위를 굴림 버튼
    public float rollingDuration = 0.5f;    // 굴러가는 시간
    public float swapInterval = 0.5f;   // 이미지 스왑 간격

    [Header("레시피 시스템")]
    public RecipeManager recipeManager; // 레시피 매니저 참조

    [Header("레시피 시스템")]
    public Vector2 dicePos; //주사위가 이동할때 목표 위치
    public float shrinkDuration = 0.5f; //크기 속도 및 이동 시간

    private bool isRolling = false; //현재 주사위가 굴러가고 있는지 확인
    private RectTransform diceRectTransform;    // 주사위의 RectTransform

    private Vector2 originalSize = new Vector2(450, 450); // 원래 크기
    private Vector2 reducedSize = new Vector2(112.5f, 112.5f);  // 축소된 크기

    void Start()
    {
        // 컴포넌트 참조
        diceRectTransform = diceImage.GetComponent<RectTransform>();

        // 버튼 클릭 이벤트 연결
        if (rollButton != null)
        {
            rollButton.onClick.AddListener(Rolling);
        }

        // 시작 시 주사위 초기화
        if (diceImage != null && diceSprites.Length > 0)
        {
            diceImage.sprite = diceSprites[0];
            // 초기 위치와 크기 설정
            diceRectTransform.anchoredPosition = Vector2.zero;
            diceRectTransform.sizeDelta = reducedSize;
        }
    }

    public void Rolling()
    {
        // 이미 굴러가고 있다면 무시
        if (isRolling) return;

        // 코루틴 시작
        StartCoroutine(RollDiceCoroutine());
    }


    IEnumerator RollDiceCoroutine()
    {
        isRolling = true;

        // 버튼 비활성화 (연속 클릭 방지)
        if (rollButton != null)
        {
            rollButton.interactable = false;
        }

        // 주사위를 (0,0) 위치로 이동하고 450x450으로 크기 변경
        diceRectTransform.anchoredPosition = Vector2.zero;
        diceRectTransform.sizeDelta = reducedSize;

        // 굴러가는 효과
        float elepsedTime = 0f;
        while (elepsedTime < rollingDuration)
        {
            //랜덤하게 주사위 이미지 변경
            int randomIndex = Random.Range(0, diceSprites.Length);
            diceImage.sprite = diceSprites[randomIndex];

            elepsedTime += swapInterval;
            yield return new WaitForSeconds(swapInterval);
        }

        // 최종 랜덤 결과 표시
        int finalResult = Random.Range(0, diceSprites.Length);
        diceImage.sprite = diceSprites[finalResult];

        // 주사위 결과 (1~6)
        int diceNumber = finalResult + 1;
        Debug.Log($"주사위 결과: {diceNumber}");

        // 레시피 생성
        if (recipeManager != null)
        {
            recipeManager.GenerateRecipe(diceNumber);
        }

        //크기를 줄이면서 dicePos로 이동하는 코루틴 시작
        yield return StartCoroutine(ShrinkAndMoveToDicePos());

        //버튼 다시 활성화
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

            // EaseOut 효과를 위한 보간 (더 자연스러운 움직임)
            float smoothT = 1 - Mathf.Pow(1 - t, 2);

            // 위치와 크기를 점진적으로 변경
            diceRectTransform.anchoredPosition = Vector2.Lerp(startPosition, dicePos, smoothT);
            diceRectTransform.sizeDelta = Vector2.Lerp(startSize, reducedSize, smoothT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치와 크기 설정 (보정)
        diceRectTransform.anchoredPosition = dicePos;
        diceRectTransform.sizeDelta = reducedSize;
    }
}
