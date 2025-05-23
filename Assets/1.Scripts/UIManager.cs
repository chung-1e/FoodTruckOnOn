using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("¹öÆ° ¼³Á¤")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button SettingButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button businessStartButton;

    [Header("¾À ÀÌ¸§")]
    [SerializeField] private string gameSceneName = "MapScene";
    [SerializeField] private string test = "Setting";
    [SerializeField] private string test1 = "Exit";
    [SerializeField] private string test2 = "IngameSceme";
    
    [Header("·©Å· UI")]
    [SerializeField] private GameObject rankingPopup;

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

    public void Setting()
    {
        SceneManager.LoadScene(test);
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
public void OnBusinessStart()
{
    SceneManager.LoadScene(test2);
}

}
