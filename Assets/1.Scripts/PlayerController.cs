using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private bool isHoldingInredient = false;        // ��Ḧ ��� �ִٴ� �� false = �ȵ�� �մ�
    private GameObject heldIngredient;             // ��� �ִ� ��� ������Ʈ

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
            Destroy(heldIngredient);

            heldIngredient = null;
            isHoldingInredient = false;

            Debug.Log("��Ḧ ����߷Ƚ��ϴ�!");
        }
    }
}
