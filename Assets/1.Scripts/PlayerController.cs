using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isHoldingInredient = false;        // ��Ḧ ��� �ִٴ� �� false = �ȵ�� �մ�
    private GameObject heldIngredient;             // ��� �ִ� ��� ������Ʈ

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

        // Animator ������Ʈ ��������
        animator = GetComponent<Animator>();
        // SpriteRenderer ������Ʈ ��������
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D ������Ʈ�� �÷��̾ �����ϴ�!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator ������Ʈ�� �÷��̾ �����ϴ�!");
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer ������Ʈ�� �÷��̾ �����ϴ�!");
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
            if (!isHoldingInredient)    // ��Ḧ ��� �մ�
            {
                TryPickUpIngredient();
            }
            else                                    // ��Ḧ ��� ���� �ʴ�
            {
                TryPlaceIngredient();
            }
        }

        if (isHoldingInredient && heldIngredient != null)
        {
            heldIngredient.transform.position = transform.position + new Vector3(0, 0.5f, 0);   // ��Ḧ ��� ������ �÷��̾�� �ణ ���� �ִ�
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
                        Debug.Log("���̵� �޴� " + station.GetIngredientName() + "��(��) �������ϴ�!");
                    }
                    else
                    {
                        Debug.Log(station.GetIngredientName() + "��(��) �������ϴ�!");
                    }

                    return;
                }
            }
        }
    }

    void UpdateAnimation(float moveX, float moveY)
    {
        // �̵� ���� Ȯ��
        isMoving = moveX != 0 || moveY != 0;

        // �ȱ� ���� ����
        animator.SetBool(is_walking, isMoving);

        // ��� ���� �ִϸ��̼� ��Ȱ��ȭ
        animator.SetBool(w_idle, false);
        animator.SetBool(ad_idle, false);
        animator.SetBool(s_idle, false);
        animator.SetBool(w_walk, false);
        animator.SetBool(ad_walk, false);
        animator.SetBool(s_walk, false);

        // �̵� ���� ��쿡�� ���� ������Ʈ
        if (isMoving)
        {
            // �켱������ ���� ���� ����
            // ���� �̵��� �켱������ ���� ��� (��/�� ������ ����)
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                lastDirection = new Vector2(moveX, 0).normalized;

                // �¿� ���� ó�� (�ݴ�� ����)
                if (moveX < 0)
                {
                    // �������� �̵� �� ��������Ʈ ����
                    spriteRenderer.flipX = false;
                }
                else
                {
                    // ���������� �̵� �� ��������Ʈ ����
                    spriteRenderer.flipX = true;
                }
            }
            // ���� �̵��� ������ ���
            else
            {
                lastDirection = new Vector2(0, moveY).normalized;
            }

            // ���� ����� �̵� ���¿� ���� ������ �ִϸ��̼� ����
            if (lastDirection.y > 0)
            {
                // ���� ����
                if (isMoving)
                {
                    animator.SetBool(w_walk, true); // �������� �ȱ�
                }
                else
                {
                    animator.SetBool(w_idle, true); // ������ �ٶ󺸸� ����
                }
            }
            else if (lastDirection.y < 0)
            {
                // �Ʒ��� ����
                if (isMoving)
                {
                    animator.SetBool(s_walk, true); // �Ʒ������� �ȱ�
                }
                else
                {
                    animator.SetBool(s_idle, true); // �Ʒ����� �ٶ󺸸� ����
                }
            }
            else if (lastDirection.x != 0)
            {
                // �¿� ����
                if (isMoving)
                {
                    animator.SetBool(ad_walk, true); // �¿�� �ȱ�
                }
                else
                {
                    animator.SetBool(ad_idle, true); // �¿츦 �ٶ󺸸� ����
                }
            }
            // �¿� ���� üũ (�̵� ���� �ƴ� ���� ������ ���⿡ ���� ���� ����) (�ݴ�� ����)
            if (lastDirection.x < 0)
            {
                spriteRenderer.flipX = false; // ������ �ٶ� �� ��������Ʈ ����
            }
            else
            {
                spriteRenderer.flipX = true; // �������� �ٶ� �� ��������Ʈ ����
            }
        }
        else
        {
            // �⺻ ���� (ó�� ���� ��)
            animator.SetBool(s_idle, true); // �⺻������ �Ʒ� ���� ����
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
                    Debug.Log("���̵� �޴��� ������ ���� ���ҽ��ϴ�!");
                }
                else
                {
                    Debug.Log("�����뿡 ��Ḧ �÷Ƚ��ϴ�!");
                }

                heldIngredient = null;
                isHoldingInredient = false;
                placedSuccessfully = true;
                break;
            }
        }

        if (!placedSuccessfully)        // �����뿡 ���� ������ �ı��ϰ� �ƹ��͵� ��� ���� ���� ���·� �ʱ�ȭ��
        {
            // ������ �� �ٸ� ���� ��� �Ǵ� ���̵� �޴��� ������
            Ingredient ingredient = heldIngredient.GetComponent<Ingredient>();
            if (ingredient != null && ingredient.isSideMenu)
            {
                Debug.Log("�̰��� �����밡 �ƴմϴ�!");
            }
            else
            {
                Debug.Log("�̰��� �����밡 �ƴմϴ�!");
            }

            // ��� ���� ������Ʈ �ı�
            Destroy(heldIngredient);
            heldIngredient = null;
            isHoldingInredient = false;
        }
    }
}
