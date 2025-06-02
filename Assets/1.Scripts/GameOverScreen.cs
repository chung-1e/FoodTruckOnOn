using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public GameObject nicknamePopup;
    public GameObject Player;

    public void RestartButton()
    {
        SceneManager.LoadScene("IngameScene");
        Player.SetActive(true);
        AudioManager.Instance.PlaySFX("마우스 클릭");
    }

    public void RankingButton()
    {
        nicknamePopup.SetActive(true); // 닉네임 팝업 띄우기
        AudioManager.Instance.PlaySFX("마우스 클릭");
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MapScene");
        AudioManager.Instance.PlaySFX("마우스 클릭");
    }
}