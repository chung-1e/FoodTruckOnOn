using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 이동에 필요

public class MapCarousel : MonoBehaviour
{
    [Header("맵 이미지들")]
    public Sprite[] mapSprites;

    [Header("맵 프리팹 및 부모")]
    public GameObject mapItemPrefab;
    public Transform mapContainer;

    [Header("버튼")]
    public Button leftButton;
    public Button rightButton;

    [Header("장사시작 버튼")]
    public Button startButton;

    [Header("맵 잠금 여부")]
    public bool[] mapUnlocked;

    [Header("이동할 인게임 씬 이름")]
    public string ingameSceneName = "InGameSceme"; 

    [Header("애니메이션 설정")]
    public float transitionDuration = 0.5f; // 전환 애니메이션 시간
    public float clickScaleDuration = 0.2f; // 클릭 애니메이션 시간
    public float clickScaleAmount = 1.1f; // 클릭 시 스케일 배율

    private List<GameObject> mapItems = new List<GameObject>();
    private int currentIndex = 0;
    private Vector2 centerSize = new Vector2(600, 600);
    private Vector2 sideSize = new Vector2(480, 480);
    private bool isAnimating = false; // 애니메이션 중 중복 입력 방지

   void Start()
{
    leftButton.onClick.AddListener(() => {
        AudioManager.Instance.PlaySFX("마우스 클릭");
        ChangeMap(-1);
    });

    rightButton.onClick.AddListener(() => {
        AudioManager.Instance.PlaySFX("마우스 클릭");
        ChangeMap(1);
    });

    startButton.onClick.AddListener(() => {
    AudioManager.Instance.PlaySFX("마우스 클릭");
    OnStartButtonClicked();
    });

    foreach (Sprite sprite in mapSprites)
    {
        CreateMap(sprite);
    }

    UpdateMapDisplay();
}



    void CreateMap(Sprite sprite)
    {
        GameObject item = Instantiate(mapItemPrefab, mapContainer);
        item.GetComponent<Image>().sprite = sprite;
        item.GetComponent<Image>().color = Color.white;

        item.transform.localScale = Vector3.one;
        mapItems.Add(item);

        // 맵 아이템 클릭 이벤트 추가
        Button mapButton = item.GetComponent<Button>();
        if (mapButton == null)
        {
            mapButton = item.AddComponent<Button>();
        }
    
        int index = mapItems.Count - 1; //현재 아이템의 인덱스
        mapButton.onClick.AddListener(() => OnMapItemClicked(index));
    }

    // 맵 아이템 클릭 시 애니메이션

    void OnMapItemClicked(int clickedIndex)
    {
        if (isAnimating) return;

        // 클릭된 맵으로 이동
        if (clickedIndex != currentIndex)
        {
            currentIndex =clickedIndex;
            StartCoroutine(UpdateMapDisplayAnimated());
        }
        else
        {
            // 현재 선택된 맵을 클릭했을 때 - 스케일 애니메이션
            StartCoroutine(PlayClickAnimation(mapItems[currentIndex]));
        }
    }
    
    
    void ChangeMap(int direction)
    {
        if (isAnimating) return; // 애니메이션 중이면 입력 무시
        
        currentIndex = (currentIndex + direction + mapItems.Count) % mapItems.Count;
        StartCoroutine(UpdateMapDisplayAnimated()); // 애니메이션으로 업데이트
    }

    // 애니메이션 없는 즉시 업데이트
    void UpdateMapDisplay()
    {
        for (int i = 0; i < mapItems.Count; i++)
        {
            RectTransform rt = mapItems[i].GetComponent<RectTransform>();

            if (i == currentIndex)
            {
                rt.sizeDelta = centerSize;
                rt.localPosition = new Vector3(0, 0, 0);
                mapItems[i].transform.SetAsLastSibling();
            }
            else if (i == (currentIndex + 1) % mapItems.Count)
            {
                rt.sizeDelta = sideSize;
                rt.localPosition = new Vector3(480, 0, 0);
            }
            else if (i == (currentIndex - 1 + mapItems.Count) % mapItems.Count)
            {
                rt.sizeDelta = sideSize;
                rt.localPosition = new Vector3(-480, 0, 0);
            }
            else
            {
                rt.localPosition = new Vector3(2000, 0, 0);
            }
        }
    }

    // 애니메이션과 함께 업데이트
System.Collections.IEnumerator UpdateMapDisplayAnimated()
{
    isAnimating = true;
    SetButtonsInteractable(false); // 버튼 비활성화

    // 각 맵 아이템의 시작 값들 저장
    List<Vector3> startPositions = new List<Vector3>();
    List<Vector2> startSizes = new List<Vector2>();
    List<Vector3> targetPositions = new List<Vector3>();
    List<Vector2> targetSizes = new List<Vector2>();

    for (int i = 0; i < mapItems.Count; i++)
    {
        RectTransform rt = mapItems[i].GetComponent<RectTransform>();
        startPositions.Add(rt.localPosition);
        startSizes.Add(rt.sizeDelta);

        Vector3 targetPosition;
        Vector2 targetSize;

        if (i == currentIndex)
        {
            targetPosition = new Vector3(0, 0, 0);
            targetSize = centerSize;
            mapItems[i].transform.SetAsLastSibling();
        }
        else if (i == (currentIndex + 1) % mapItems.Count)
        {
            targetPosition = new Vector3(480, 0, 0);
            targetSize = sideSize;
        }
        else if (i == (currentIndex - 1 + mapItems.Count) % mapItems.Count)
        {
            targetPosition = new Vector3(-480, 0, 0);
            targetSize = sideSize;
        }
        else
        {
            targetPosition = new Vector3(2000, 0, 0);
            targetSize = sideSize;
        }

        targetPositions.Add(targetPosition);
        targetSizes.Add(targetSize);
    }

    // 애니메이션 실행
    float elapsedTime = 0f;
    while (elapsedTime < transitionDuration)
    {
        float t = elapsedTime / transitionDuration;
        // 이징 함수 적용 (OutCubic)
        t = 1f - Mathf.Pow(1f - t, 3f);

        for (int i = 0; i < mapItems.Count; i++)
        {
            RectTransform rt = mapItems[i].GetComponent<RectTransform>();
            rt.localPosition = Vector3.Lerp(startPositions[i], targetPositions[i], t);
            rt.sizeDelta = Vector2.Lerp(startSizes[i], targetSizes[i], t);
        }

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // 최종 위치 보정
    for (int i = 0; i < mapItems.Count; i++)
    {
        RectTransform rt = mapItems[i].GetComponent<RectTransform>();
        rt.localPosition = targetPositions[i];
        rt.sizeDelta = targetSizes[i];
    }

    isAnimating = false;
    SetButtonsInteractable(true); // 버튼 재활성화
}

// 클릭 애니메이션 (스케일 효과)
System.Collections.IEnumerator PlayClickAnimation(GameObject target)
{
    Transform targetTransform = target.transform;
    Vector3 originalScale = targetTransform.localScale;
    Vector3 targetScale = originalScale * clickScaleAmount;

    // 확대 애니메이션
    float elapsedTime = 0f;
    float halfDuration = clickScaleDuration * 0.5f;

    while (elapsedTime < halfDuration)
    {
        float t = elapsedTime / halfDuration;
        // OutQuad 이징
        t = 1f - (1f - t) * (1f - t);
        targetTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // 축소 애니메이션
    elapsedTime = 0f;
    while (elapsedTime < halfDuration)
    {
        float t = elapsedTime / halfDuration;
        // InQuad 이징
        t = t * t;
        targetTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    targetTransform.localScale = originalScale;
}

// 잠긴 맵 클릭 시 흔들림 애니메이션
System.Collections.IEnumerator PlayLockedMapAnimation(GameObject target)
{
    Transform targetTransform = target.transform;
    Vector3 originalPosition = targetTransform.localPosition;
    float shakeIntensity = 20f;
    float shakeDuration = 0.5f;

    float elapsedTime = 0f;
    while (elapsedTime < shakeDuration)
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-shakeIntensity, shakeIntensity),
            Random.Range(-shakeIntensity, shakeIntensity),
            0f
        );

        targetTransform.localPosition = originalPosition + randomOffset;
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    targetTransform.localPosition = originalPosition;
}

// 버튼 활성화/비활성화
void SetButtonsInteractable(bool interactable)
{
    leftButton.interactable = interactable;
    rightButton.interactable = interactable;
    startButton.interactable = interactable;
}

void OnStartButtonClicked()
{
    
    if (isAnimating) return; // 애니메이션 중이면 무시

    if (mapUnlocked.Length > currentIndex && mapUnlocked[currentIndex])
    {
        // 선택된 맵에 클릭 애니메이션 적용 후 씬 이동
        StartCoroutine(StartGameWithAnimation());
    }
    else
    {
        Debug.Log("이 맵은 잠겨있습니다.");
        // 잠긴 맵 클릭 시 흔들림 애니메이션
        StartCoroutine(PlayLockedMapAnimation(mapItems[currentIndex]));
    }
}

// 게임 시작 애니메이션
System.Collections.IEnumerator StartGameWithAnimation()
{
    yield return StartCoroutine(PlayClickAnimation(mapItems[currentIndex]));
    Debug.Log("게임 시작! 선택된 맵: " + currentIndex);
    SceneManager.LoadScene(ingameSceneName);
}
}
