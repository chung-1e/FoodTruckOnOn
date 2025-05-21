using UnityEngine;

public class NicknameValidator : MonoBehaviour
{
    public GameObject nicknamePopup;
    public TMPro.TMP_InputField nicknameInputField;

   

    public void OnNicknameConfirm()
    {
        string nickname = nicknameInputField.text.Trim();
        if (string.IsNullOrEmpty(nickname))
        {
            Debug.Log("�г����� �Է����ּ���.");
            return;
        }

        Debug.Log("�г��� �Է� �Ϸ�: " + nickname);
        // �г��� �����ϰų� ��ŷ�� ����ϴ� �ڵ� �ֱ�

        nicknamePopup.SetActive(false);
    }
}