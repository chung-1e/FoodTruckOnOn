using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public GameObject nicknamePopup; 

    public void RestartButton()
    {
        SceneManager.LoadScene("IngameSceme"); 
    }

    public void RankingButton()
    {
        nicknamePopup.SetActive(true); // ´Ð³×ÀÓ ÆË¾÷ ¶ç¿ì±â
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainScene");
    }
}