using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Container : MonoBehaviour
{
    public static UI_Container Instance { get; private set; }
    [SerializeField] private Slider HP_UI;
    [SerializeField] private TextMeshProUGUI HP_Text;

    [SerializeField] private TextMeshProUGUI Gold_Text;

    [SerializeField] private GameObject[] toggleText;

    [SerializeField] private GameObject hashshin_gauge_UI;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image gaugeImage;
    float maxGauge = 100f;
    float curGauge;
    [SerializeField] private Color maxColor;

    [SerializeField] private GameObject Ability_UI;
    [SerializeField] private GameObject Esc_UI;
    [SerializeField] private GameObject Book_UI;

    float MaxHP;
    float CurHP;
    private void OnEnable()
    {
        Instance = this;
        if (PlayerPrefs.GetInt("weaponType") == 3) hashshin_gauge_UI.SetActive(true);
        curGauge = 0f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Esc_UI.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            //
            Book_UI.SetActive(true);
        }
    }

    public void HandleHP(float cur, float max)
    {
        MaxHP = max;
        CurHP = cur;
        HP_UI.value = CurHP / MaxHP;
        HP_Text.text = ((int)CurHP).ToString() + " / " + ((int)MaxHP).ToString();
    }
    public void HandleGold(int gold)
    {
        Gold_Text.text = gold.ToString();
    }
    public void EnableToggleText(int idx)
    {
        //Debug.Log("Enable toggle text : " + idx);
        toggleText[idx].GetComponent<Animator>().SetBool("IsEnabled", true);
    }
    public void DisableToggleText(int idx)
    {
        //Debug.Log("Disable toggle text : " + idx);
        toggleText[idx].GetComponent<Animator>().SetBool("IsEnabled", false);
    }
    public void updateGauge(float gauge)
    {
        curGauge = gauge;
        gaugeImage.fillAmount = curGauge / maxGauge;
        if (curGauge == maxGauge)
        {
            borderImage.color = maxColor;
        }
        else borderImage.color = new Color(255f, 255f, 255f);
    }
    public void GiveUp()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(0);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        //Debug.Log("quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
