using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

        // rankingManager¿¡¼­ ·©Å· Á¤º¸ ºÒ·¯¿Í Áßº¹ È®ÀÎ
        if (RankingManager.Instance != null)
        {
           var ranks = RankingManager.Instance.GetAllRanks();

            foreach (var entry in ranks)
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
         AudioManager.Instance.PlaySFX("¸¶¿ì½º Å¬¸¯"); 
        string nickname = nicknameInputField.text.Trim();
        Debug.Log("´Ð³×ÀÓ ÀúÀåµÊ: " + nickname);

        PlayerPrefs.SetString("PlayerNickname", nickname);
        PlayerPrefs.Save();
    
      //·©Å·¿¡ µî·Ï (ScoreManager¿¡¼­ Á¡¼ö¿Í ÇÃ·¹ÀÌ ½Ã°£ °¡Á®¿À±â)
      if (RankingManager.Instance != null && ScoreManager.Instance != null)
      {
          RankingManager.Instance.AddRank(nickname,
          ScoreManager.Instance.currentScore, ScoreManager.Instance.playTime);
      }
      
      //·©Å· ¾ÀÀ¸·Î ÀÌµ¿
      SceneManager.LoadScene("MapScene");

    }
    

}
