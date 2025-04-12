using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isHoldingInredient = false;        // 재료를 잡고 있다는 뜻 false = 안들고 잇다
    private GameObject heldIngredient;             // 잡고 있는 재료 오브젝트

    private bool isMoving = false;
    private Vector2 lastDirection = Vector2.down;

    private const string w_idle = "w_idle";
    private const string ad_idle = "ad_idle";
    private const string s_idle = "s_idle";

    private const string w_walk = "w_walk";
    private const string ad_walk = "ad_walk";
    private const string s_walk = "s_walk";

    private const string is_walking = "isWalking";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();
        // SpriteRenderer 컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D 컴포넌트가 플레이어에 없습니다!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 플레이어에 없습니다!");
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer 컴포넌트가 플레이어에 없습니다!");
        }

        animator.speed = 1.0f;
    }

    void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W))
        { moveY = 1f; }
        if (Input.GetKey(KeyCode.S))
        { moveY = -1f; }
        if (Input.GetKey(KeyCode.A))
        { moveX = -1f; }
        if (Input.GetKey(KeyCode.D))
        { moveX = 1f; }

        Vector2 movement = new Vector2(moveX, moveY).normalized * moveSpeed;

        UpdateAnimation(moveX, moveY);

        rb.velocity = movement;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!isHoldingInredient)    // 재료를 잡고 잇다
            {
                TryPickUpIngredient();
            }
            else                                    // 재료를 잡고 있지 않다
            {
                TryPlaceIngredient();
            }
        }

        if (isHoldingInredient && heldIngredient != null)
        {
            heldIngredient.transform.position = transform.position + new Vector3(0, 0.5f, 0);   // 재료를 잡고 있으면 플레이어보다 약간 위에 있다
        }
    }

    void TryPickUpIngredient()
    {
        Collider2D[] contacts = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        int numContacts = Physics2D.OverlapCollider(GetComponent<Collider2D>(), filter, contacts);

        for (int i = 0; i < numContacts; i++)
        {
            Collider2D collider = contacts[i];

            IngredientStation station = collider.GetComponent<IngredientStation>();
            if (station != null)
            {
                GameObject newIngredient = station.SpawnIngredient();
                if (newIngredient != null)
                {
                    heldIngredient = newIngredient;
                    isHoldingInredient = true;

                    Ingredient ingredient = newIngredient.GetComponent<Ingredient>();
                    if (ingredient != null && ingredient.isSideMenu)
                    {
                        Debug.Log("사이드 메뉴 " + station.GetIngredientName() + "을(를) 집었습니다!");
                    }
                    else
                    {
                        Debug.Log(station.GetIngredientName() + "을(를) 집었습니다!");
                    }

                    return;
                }
            }
        }
    }

    void UpdateAnimation(float moveX, float moveY)
    {
        // 이동 여부 확인
        isMoving = moveX != 0 || moveY != 0;

        // 걷기 상태 설정
        animator.SetBool(is_walking, isMoving);

        // 모든 방향 애니메이션 비활성화
        animator.SetBool(w_idle, false);
        animator.SetBool(ad_idle, false);
        animator.SetBool(s_idle, false);
        animator.SetBool(w_walk, false);
        animator.SetBool(ad_walk, false);
        animator.SetBool(s_walk, false);

        // 이동 중인 경우에만 방향 업데이트
        if (isMoving)
        {
            // 우선순위에 따라 방향 결정
            // 수평 이동이 우선순위가 높을 경우 (좌/우 방향이 메인)
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                lastDirection = new Vector2(moveX, 0).normalized;

                // 좌우 반전 처리 (반대로 설정)
                if (moveX < 0)
                {
                    // 왼쪽으로 이동 시 스프라이트 정상
                    spriteRenderer.flipX = false;
                }
                else
                {
                    // 오른쪽으로 이동 시 스프라이트 반전
                    spriteRenderer.flipX = true;
                }
            }
            // 수직 이동이 메인인 경우
            else
            {
                lastDirection = new Vector2(0, moveY).normalized;
            }

            // 현재 방향과 이동 상태에 따라 적절한 애니메이션 설정
            if (lastDirection.y > 0)
            {
                // 위쪽 방향
                if (isMoving)
                {
                    animator.SetBool(w_walk, true); // 위쪽으로 걷기
                }
                else
                {
                    animator.SetBool(w_idle, true); // 위쪽을 바라보며 정지
                }
            }
            else if (lastDirection.y < 0)
            {
                // 아래쪽 방향
                if (isMoving)
                {
                    animator.SetBool(s_walk, true); // 아래쪽으로 걷기
                }
                else
                {
                    animator.SetBool(s_idle, true); // 아래쪽을 바라보며 정지
                }
            }
            else if (lastDirection.x != 0)
            {
                // 좌우 방향
                if (isMoving)
                {
                    animator.SetBool(ad_walk, true); // 좌우로 걷기
                }
                else
                {
                    animator.SetBool(ad_idle, true); // 좌우를 바라보며 정지
                }
            }
            // 좌우 방향 체크 (이동 중이 아닐 때도 마지막 방향에 따라 반전 유지) (반대로 설정)
            if (lastDirection.x < 0)
            {
                spriteRenderer.flipX = false; // 왼쪽을 바라볼 때 스프라이트 정상
            }
            else
            {
                spriteRenderer.flipX = true; // 오른쪽을 바라볼 때 스프라이트 반전
            }
        }
        else
        {
            // 기본 방향 (처음 시작 시)
            animator.SetBool(s_idle, true); // 기본적으로 아래 방향 정지
        }
    }

    void TryPlaceIngredient()
    {
        if (!isHoldingInredient || heldIngredient == null)
            return;

        Collider2D[] contacts = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        int numContacts = Physics2D.OverlapCollider(GetComponent<Collider2D>(), filter, contacts);
        bool placedSuccessfully = false;

        for (int i = 0; i < numContacts; i++)
        {
            Collider2D collider = contacts[i];

            CookingStation cookingStation = collider.GetComponent<CookingStation>();

            if (cookingStation != null)
            {
                cookingStation.AddIngredient(heldIngredient);

                Ingredient ingredient = heldIngredient.GetComponent<Ingredient>();
                if (ingredient != null && ingredient.isSideMenu)
                {
                    Debug.Log("사이드 메뉴를 조리대 옆에 놓았습니다!");
                }
                else
                {
                    Debug.Log("조리대에 재료를 올렸습니다!");
                }

                heldIngredient = null;
                isHoldingInredient = false;
                placedSuccessfully = true;
                break;
            }
        }

        if (!placedSuccessfully)        // 조리대에 놓지 않으면 파괴하고 아무것도 들고 있지 않은 상태로 초기화함
        {
            // 조리대 외 다른 곳에 재료 또는 사이드 메뉴를 놓으면
            Ingredient ingredient = heldIngredient.GetComponent<Ingredient>();
            if (ingredient != null && ingredient.isSideMenu)
            {
                Debug.Log("이곳은 조리대가 아닙니다!");
            }
            else
            {
                Debug.Log("이곳은 조리대가 아닙니다!");
            }

            // 재료 게임 오브젝트 파괴
            Destroy(heldIngredient);
            heldIngredient = null;
            isHoldingInredient = false;
        }
    }
}
