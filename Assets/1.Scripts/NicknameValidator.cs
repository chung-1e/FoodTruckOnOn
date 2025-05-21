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
            Debug.Log("닉네임을 입력해주세요.");
            return;
        }

        Debug.Log("닉네임 입력 완료: " + nickname);
        // 닉네임 저장하거나 랭킹에 등록하는 코드 넣기

        nicknamePopup.SetActive(false);
    }
}