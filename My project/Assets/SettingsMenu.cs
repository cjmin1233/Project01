using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    static int GCD(int num1, int num2)
    {
        int remainder;
        while (num2 != 0)
        {
            remainder = num1 % num2;
            num1 = num2;
            num2 = remainder;
        }
        return num1;
    }
    public AudioMixer audioMixer;
    public Slider MasterAudioSlider;
    public Slider BgmAudioSlider;
    public Slider EffectAudioSlider;
    
    //public Dropdown resolutionDropdown;
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    private void Start()
    {
        MasterAudioSlider.value = PlayerPrefs.GetFloat("MaterVolume", 1f);
        BgmAudioSlider.value = PlayerPrefs.GetFloat("BgmVolume", 0.75f);
        EffectAudioSlider.value = PlayerPrefs.GetFloat("EffectVolume", 0.75f);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();


        int currentResolutionIndex = 0;
        //
        /*for (int i = 0; i < resolutions.Length; i++)
        {
            var MAX = GCD(resolutions[i].width, resolutions[i].height);
            if ((resolutions[i].width / MAX == 16) && (resolutions[i].height / MAX == 9))
            {
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutions[i].width + " X " + resolutions[i].height));
                //resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutions[i].ToString()));
            }

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }*/
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " X " + resolutions[i].height;
            if (!options.Contains(option)) {
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.IndexOf(option);
                    Debug.Log(currentResolutionIndex);
                }
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BgmVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BgmVolume", volume);
    }
    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("EffectVolume", volume);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    /*public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }*/
}
