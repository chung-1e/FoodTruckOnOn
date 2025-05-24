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

        if (!Regex.IsMatch(input, @"^[a-zA-Z0-9��-�R]+$"))
        {
            warningText.text = "�ѱ�, ����, ���ڸ� �Է��ϼ���.";
            confirmButton.interactable = false;
            return;
        }

        if (input.Length < 1 || input.Length > 16)
        {
            warningText.text = "�г����� 1~16�� ���̿��� �մϴ�.";
            confirmButton.interactable = false;
            return;
        }

        // PlayerPrefs���� ��ŷ ���� �ҷ��� �ߺ� Ȯ��
        if (PlayerPrefs.HasKey("Ranking"))
        {
            string json = PlayerPrefs.GetString("Ranking");
            RankData data = JsonUtility.FromJson<RankData>(json);

            foreach (var entry in data.ranks)
            {
                if (entry.nickname == input)
                {
                    warningText.text = "�̹� ��� ���� �г����Դϴ�.";
                    confirmButton.interactable = false;
                    return;
                }
            }
        }

        // �ϵ��ڵ��� �ߺ� �г��� �˻�
        if (usedNicknames.Contains(input))
        {
            warningText.text = "�̹� ��� ���� �г����Դϴ�.";
            confirmButton.interactable = false;
            return;
        }

        confirmButton.interactable = true;
    }

    void OnConfirm()
    {
        string nickname = nicknameInputField.text.Trim();
        Debug.Log("�г��� �����: " + nickname);

        PlayerPrefs.SetString("PlayerNickname", nickname);
        PlayerPrefs.Save();

        popupPanel.SetActive(false);
    }
}
