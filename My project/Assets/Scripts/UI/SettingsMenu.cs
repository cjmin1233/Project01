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
    
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreenToggle;
    Resolution[] resolutions;
    List<Resolution> availableResolutions;

    public GameObject LoadButton;

    [SerializeField] private GameObject Fade_UI;
    private void Start()
    {
        #region 초기 옵션 세팅
        fullScreenToggle.isOn = Screen.fullScreen;

        if (!PlayerPrefs.HasKey("MasterVolume")) SetMasterVolume(1f);
        MasterAudioSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        if (!PlayerPrefs.HasKey("BgmVolume")) SetBgmVolume(1f);
        BgmAudioSlider.value = PlayerPrefs.GetFloat("BgmVolume");
        if (!PlayerPrefs.HasKey("EffectVolume")) SetEffectVolume(1f);
        EffectAudioSlider.value = PlayerPrefs.GetFloat("EffectVolume");

        availableResolutions = new List<Resolution>();
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
                availableResolutions.Add(resolutions[i]);
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.IndexOf(option);
                }
            }
        }
        resolutionDropdown.AddOptions(options);
        if (!PlayerPrefs.HasKey("ResolutionIndex")) PlayerPrefs.SetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex");
        resolutionDropdown.RefreshShownValue();
        #endregion

        // 게임 데이터 로드 후 이어하기 버튼 활성화
        DataManager.Instance.LoadGameData();
        Data gameData = DataManager.Instance.data;
        if (gameData.weaponType > 0)
        {
            // 데이터가 남아있는 경우 '이어하기' 활성화
            LoadButton.GetComponent<Button>().interactable = true;
            LoadButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(255f, 255f, 255f);
        }
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
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
    public void PlayGame(int selected)
    {
        StartCoroutine(FadeFlow(selected));
    }
    public void CloseGame()
    {
        GameManager.Instance.CloseGame();
    }
    public IEnumerator FadeFlow(int selected)
    {
        Fade_UI.SetActive(true);
        Image fadeImage = Fade_UI.GetComponent<Image>();
        for (float f = 0f; f < 1f; f += Time.deltaTime * 2)
        {
            Color c = fadeImage.color;
            c.a = f;
            fadeImage.color = c;
            yield return null;
        }
        GameManager.Instance.PlayGame(selected);
    }

}
