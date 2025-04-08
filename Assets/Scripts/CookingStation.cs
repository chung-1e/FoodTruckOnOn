using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    private List<GameObject> placedIngredients = new List<GameObject>();

    public float stackHeight = 0.2f;

    public bool HasAnyIngredients()
    {
        return placedIngredients.Count > 0;     // 조리대에 재료가 하나라도 있으면 참
    }

    public void AddIngredient(GameObject ingredient)
    {
        if (ingredient == null)
            return;

        placedIngredients.Add(ingredient);

        ingredient.transform.SetParent(transform);

        StackIngredients();
    }

    private void StackIngredients()
    {
        Vector3 basePosition = transform.position;
        float currentHeight = 0f;
        for (int i = 0; i < placedIngredients.Count; i++)
        {
            GameObject ingredient = placedIngredients[i];
            Vector3 newPosition = new Vector3(
                basePosition.x,
                basePosition.y + currentHeight,
                basePosition.z);
            ingredient.transform.position = newPosition;
            currentHeight += stackHeight;
        }
    }

    public void ClearStation()
    {
        foreach (GameObject ingredient in placedIngredients)
        {
            Destroy(ingredient);
        }

        placedIngredients.Clear();
    }
}
