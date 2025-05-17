using UnityEngine;
using UnityEngine.SceneManagement;

public class MyButton : MonoBehaviour
{
    public void OnRetry()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    public void OnRanking()
    {
        SceneManager.LoadScene("RankingScene"); // 랭킹 씬으로 이동
    }

    public void OnExit()
    {
        SceneManager.LoadScene("MainScene"); // 메인으로 돌아가기
    }
}