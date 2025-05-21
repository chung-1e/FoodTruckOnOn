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
        nicknamePopup.SetActive(true); // �г��� �˾� ����
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainScene");
    }
}