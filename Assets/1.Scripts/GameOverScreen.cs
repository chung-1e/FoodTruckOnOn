using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public GameObject nicknamePopup; 

    public void RestartButton()
    {
        SceneManager.LoadScene("IngameScene"); 
         AudioManager.Instance.PlaySFX("마우스 클릭"); 
    }

    public void RankingButton()
    {
        nicknamePopup.SetActive(true); // 닉네임 팝업 띄우기
         AudioManager.Instance.PlaySFX("마우스 클릭"); 
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainScene");
         AudioManager.Instance.PlaySFX("마우스 클릭"); 
    }
}