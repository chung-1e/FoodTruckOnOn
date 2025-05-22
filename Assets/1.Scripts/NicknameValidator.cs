using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NicknameValidator : MonoBehaviour
{
    public TMP_InputField nicknameInputField;
    public TMP_Text warningText;
    public Button confirmButton;
    public GameObject popupPanel; // ÆË¾÷ ÀüÃ¼ ÆĞ³Î

    // ¿¹½Ã: ÀÌ¹Ì µî·ÏµÈ ´Ğ³×ÀÓ ¸ñ·Ï
    private HashSet<string> usedNicknames = new HashSet<string>() { "Player1", "admin", "tester" };

    void Start()
    {
        nicknameInputField.onValueChanged.AddListener(ValidateNickname);
        confirmButton.onClick.AddListener(OnConfirm);
    }

    void ValidateNickname(string input)
    {
        warningText.text = "";
        input = input.Trim();

        if (!Regex.IsMatch(input, @"^[a-zA-Z0-9°¡-ÆR]+$"))
        {
            warningText.text = "ÇÑ±Û, ¿µ¾î, ¼ıÀÚ¸¸ ÀÔ·ÂÇÏ¼¼¿ä.";
            confirmButton.interactable = false;
            return;
        }

        if (input.Length < 1 || input.Length > 16)
        {
            warningText.text = "´Ğ³×ÀÓÀº 1~16ÀÚ »çÀÌ¿©¾ß ÇÕ´Ï´Ù.";
            confirmButton.interactable = false;
            return;
        }

        if (usedNicknames.Contains(input))
        {
            warningText.text = "ÀÌ¹Ì »ç¿ë ÁßÀÎ ´Ğ³×ÀÓÀÔ´Ï´Ù.";
            confirmButton.interactable = false;
            return;
        }

        confirmButton.interactable = true;
    }

    void OnConfirm()
    {
        string nickname = nicknameInputField.text.Trim();
        Debug.Log("´Ğ³×ÀÓ ÀúÀåµÊ: " + nickname);

        // ÀúÀå ·ÎÁ÷ ¿©±â¿¡ Ãß°¡
        // usedNicknames.Add(nickname);

        // ÆË¾÷ ´İ±â
        popupPanel.SetActive(false);
    }
}
