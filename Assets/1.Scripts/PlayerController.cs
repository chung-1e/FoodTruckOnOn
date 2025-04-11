using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private bool isHoldingInredient = false;        // 재료를 잡고 있다는 뜻 false = 안들고 잇다
    private GameObject heldIngredient;             // 잡고 있는 재료 오브젝트

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            Destroy(heldIngredient);

            heldIngredient = null;
            isHoldingInredient = false;

            Debug.Log("재료를 떨어뜨렸습니다!");
        }
    }
}
