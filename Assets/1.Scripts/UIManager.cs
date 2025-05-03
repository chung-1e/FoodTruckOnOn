using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("버튼설정")]
    [SerializeField] private Button startButton;    // 시작 버튼
    [SerializeField] private Button SettingButton;    // 세팅 버튼
    [SerializeField] private Button ExitButton;    // 나가기 버튼
    // 넘어갈 씬의 이름
    [SerializeField] private string gameSceneName = "IngameSceme";
    [SerializeField] private string test = "Setting";
    [SerializeField] private string test1 = "Exit";
    
    void Awake()
    {
        if (startButton == null)
        {
            startButton = GetComponent<Button>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 게임 씬 로드
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
}
