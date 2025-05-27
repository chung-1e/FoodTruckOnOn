using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        slider.value = savedVolume;

        slider.onValueChanged.AddListener(SetVolume);
    }

    void SetVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
            PlayerPrefs.SetFloat("MasterVolume", value);
        }
    }
}
