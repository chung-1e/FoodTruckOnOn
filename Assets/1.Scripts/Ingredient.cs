using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public IngredientStation.IngredientType type;

    // 유저가 잡은 재료가 사이드 메뉴인지의 여부
    public bool isSideMenu = false;
}
