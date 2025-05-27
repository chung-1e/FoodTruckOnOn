using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private string currentMusicName = "";

     public AudioMixer audioMixer; 


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlaySceneMusic(SceneManager.GetActiveScene().name);

        // 저장된 값 불러오기
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(savedVolume);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic(scene.name);
    }

    private void PlaySceneMusic(string sceneName)
{
    string musicToPlay = "";

    // 메인과 맵 씬은 같은 음악으로 취급
    if (sceneName == "MainScene" || sceneName == "MapScene")
    {
        musicToPlay = "메인화면2";
    }
    else if (sceneName == "IngameSceme")
    {
        musicToPlay = "게임 BGM";
    }
    else
    {
        musicToPlay = ""; // 다른 씬에서는 음악 정지
    }

    if (string.IsNullOrEmpty(musicToPlay))
    {
        StopMusic();
        currentMusicName = "";
    }
    else
    {
        // 피버타임이 아닐 때만 정상 BGM 재생
        if (!isFeverTime)
        {
            PlayMusic(musicToPlay);
            currentMusicName = musicToPlay;
        }
    }
}


    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("소리를 찾을 수 없음: " + name);
            return;
        }

        musicSource.clip = s.clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("소리를 찾을 수 없음: " + name);
            return;
        }

        sfxSource.PlayOneShot(s.clip);
    }
     private bool isFeverTime = false;

    // 피버타임 시작 시 호출
    public void StartFeverTime()
    {
        if (isFeverTime) return; // 이미 피버타임 중이면 무시
        isFeverTime = true;

        PlayMusic("피버타임 BGM");  // 피버타임 BGM 이름에 맞게 바꿔주세요
    }

    // 피버타임 종료 시 호출
    public void EndFeverTime()
    {
        if (!isFeverTime) return;
        isFeverTime = false;

        // 씬에 따라 원래 BGM으로 복귀
        string currentScene = SceneManager.GetActiveScene().name;
        PlaySceneMusic(currentScene);
    }

    


      public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }

    
}
