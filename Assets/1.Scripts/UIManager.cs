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

    [Header("씬 이름")]
    [SerializeField] private string gameSceneName = "MapScene";
    [SerializeField] private string test1 = "Exit";
   
    
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
        SceneManager.LoadScene(gameSceneName);
    }

    public void Exit()
    {
        SceneManager.LoadScene(test1);
    }

    public void OpenRankingPopup()
    {
        if (rankingPopup != null)
            rankingPopup.SetActive(true);
    }

    public void CloseRankingPopup()
    {
        if (rankingPopup != null)
            rankingPopup.SetActive(false);
    }

    public void OpenSettingPopup()
    {
         if (settingPopup != null)
            settingPopup.SetActive(true);
    }
     public void CloseSettingPopup()
    {
        if (settingPopup != null)
            settingPopup.SetActive(false);
    }


}
