using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("��ư����")]
    [SerializeField] private Button startButton;    // ���� ��ư
    [SerializeField] private Button SettingButton;    // ���� ��ư
    [SerializeField] private Button ExitButton;    // ������ ��ư
    // �Ѿ ���� �̸�
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

    // ���� �� �ε�
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
