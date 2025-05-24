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
    public GameObject popupPanel;

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
            warningText.text = "ÇÑ±Û, ¿µ¾î, ¼ýÀÚ¸¸ ÀÔ·ÂÇÏ¼¼¿ä.";
            confirmButton.interactable = false;
            return;
        }

        if (input.Length < 1 || input.Length > 16)
        {
            warningText.text = "´Ð³×ÀÓÀº 1~16ÀÚ »çÀÌ¿©¾ß ÇÕ´Ï´Ù.";
            confirmButton.interactable = false;
            return;
        }

        // PlayerPrefs¿¡¼­ ·©Å· Á¤º¸ ºÒ·¯¿Í Áßº¹ È®ÀÎ
        if (PlayerPrefs.HasKey("Ranking"))
        {
            string json = PlayerPrefs.GetString("Ranking");
            RankData data = JsonUtility.FromJson<RankData>(json);

            foreach (var entry in data.ranks)
            {
                if (entry.nickname == input)
                {
                    warningText.text = "ÀÌ¹Ì »ç¿ë ÁßÀÎ ´Ð³×ÀÓÀÔ´Ï´Ù.";
                    confirmButton.interactable = false;
                    return;
                }
            }
        }

        // ÇÏµåÄÚµùµÈ Áßº¹ ´Ð³×ÀÓ °Ë»ç
        if (usedNicknames.Contains(input))
        {
            warningText.text = "ÀÌ¹Ì »ç¿ë ÁßÀÎ ´Ð³×ÀÓÀÔ´Ï´Ù.";
            confirmButton.interactable = false;
            return;
        }

        confirmButton.interactable = true;
    }

    void OnConfirm()
    {
        string nickname = nicknameInputField.text.Trim();
        Debug.Log("´Ð³×ÀÓ ÀúÀåµÊ: " + nickname);

        PlayerPrefs.SetString("PlayerNickname", nickname);
        PlayerPrefs.Save();

        popupPanel.SetActive(false);
    }
}
