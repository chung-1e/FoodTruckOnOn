using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("버튼 설정")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button SettingButton;
    [SerializeField] private Button HomeButton;

    [Header("씬 이름")]
    [SerializeField] private string gameSceneName = "MapScene";
    [SerializeField] private string test2 = "MainScene";
   
    
    [Header("랭킹 UI")]
    [SerializeField] private GameObject rankingPopup;

    [Header("소리설정")]
    [SerializeField] private GameObject settingPopup;
   
    void Awake()
    {
        if (startButton == null)
        {
            startButton = GetComponent<Button>();
        }
    }

   public void StartGame()
{
    AudioManager.Instance.PlaySFX("마우스 클릭"); 
    SceneManager.LoadScene(gameSceneName);
}

public void Exit()
{
    AudioManager.Instance.PlaySFX("마우스 클릭");
        ExitGame();
}

public void Home()
{
    AudioManager.Instance.PlaySFX("마우스 클릭"); 
    SceneManager.LoadScene(test2);
}

public void OpenRankingPopup()
{
    AudioManager.Instance.PlaySFX("마우스 클릭"); 
    if (rankingPopup != null)
        rankingPopup.SetActive(true);
}

public void CloseRankingPopup()
{
    AudioManager.Instance.PlaySFX("마우스 클릭"); 
    if (rankingPopup != null)
        rankingPopup.SetActive(false);
}

public void OpenSettingPopup()
{
    AudioManager.Instance.PlaySFX("마우스 클릭"); 
    if (settingPopup != null)
        settingPopup.SetActive(true);
}

public void CloseSettingPopup()
{
    AudioManager.Instance.PlaySFX("마우스 클릭"); 
    if (settingPopup != null)
        settingPopup.SetActive(false);
}

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
