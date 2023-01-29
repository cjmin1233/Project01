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

    // 어빌리티 UI
    [SerializeField] private GameObject Ability_UI;
    [SerializeField] private RectTransform[] buttonLocations;
    [SerializeField] private GameObject[] abilityButtons;
    [SerializeField] private List<GameObject> availableAbilityList;
    [SerializeField] private GameObject[] hiddenKnightAbility_Z;
    [SerializeField] private GameObject[] hiddenKnightAbility_X;
    [SerializeField] private GameObject[] hiddenRangerAbility_Z;
    [SerializeField] private GameObject[] hiddenRangerAbility_X;
    [SerializeField] private GameObject[] hiddenHashashinAbility_Z;

    private GameObject player;
    int totalWeight;
    int remainAbility;
    int current_weight;


    [SerializeField] private GameObject Esc_UI;
    [SerializeField] private GameObject Book_UI;

    float MaxHP;
    float CurHP;
    private void OnEnable()
    {
        Instance = this;
        if (PlayerPrefs.GetInt("weaponType") == 3) hashshin_gauge_UI.SetActive(true);
        curGauge = 0f;
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            abilityButtons[i].GetComponent<Ability>().index = i;
        }
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
    public void RandomAbility()
    {
        totalWeight = 0;
        remainAbility = 0;

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            // 남아있는 어빌리티 초기화
            abilityButtons[i].GetComponent<Ability>().isAppeared = false;
            current_weight = abilityButtons[i].GetComponent<Ability>().weight;
            // 가중치 남아있는 것들 개수 세기
            if (current_weight != 0)
            {
                totalWeight += current_weight;
                remainAbility++;
            }
        }


        if (remainAbility == 0)
        {
            Debug.Log("There's no remain ability");
        }
        else
        {
            //
            Debug.Log("Remain ability is : " + remainAbility);
            if (remainAbility > 3) remainAbility = 3;
            Ability_UI.SetActive(true);

            for (int j = 0; j < remainAbility; j++)
            {
                int rand = Random.Range(0, totalWeight);
                int weight = 0;

                for (int i = 0; i < abilityButtons.Length; i++)
                {
                    current_weight = abilityButtons[i].GetComponent<Ability>().weight;
                    if (current_weight == 0 || abilityButtons[i].GetComponent<Ability>().isAppeared)
                    {
                        continue;
                    }
                    else
                    {
                        weight += current_weight;
                        if (rand < weight)
                        {
                            rand = i;
                            Debug.Log("Random index is : " + rand);
                            break;
                        }
                    }
                }
                RectTransform rect = abilityButtons[rand].GetComponent<RectTransform>();
                rect.anchoredPosition = buttonLocations[j].anchoredPosition;
                //abilityButtons[rand].transform.position = buttonLocations[j].transform.position;
                abilityButtons[rand].SetActive(true);
                abilityButtons[rand].GetComponent<Ability>().isAppeared = true;
                totalWeight -= abilityButtons[rand].GetComponent<Ability>().weight;

            }
        }

    }
    public void GetAbility(Ability SelectedAbility)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        int index = SelectedAbility.index;
        SelectedAbility.isSelected = true;
        SelectedAbility.isAppeared = false;
        SelectedAbility.level++;
        // 선택된 능력 확인
        /*Debug.Log("Selected Ability : " + index + "th ability.");
        Debug.Log("Ability Type : " + SelectedAbility.Type);
        Debug.Log("Ability Tier : " + SelectedAbility.Tier);
        Debug.Log("Ability Level : " + SelectedAbility.level);
        Debug.Log("Ability Weight : " + SelectedAbility.weight);*/
        //*******************************************

        SelectedAbility.weight = 0;    //선택된 능력 출현 확률 0으로
        float lv = (float)SelectedAbility.level;
        // Apply ability
        if (SelectedAbility.index == 0)
        {
            // power up z attack
            player.GetComponent<PlayerAttack>().damage_z_multiplier = 1f + 0.1f * lv;
        }
        else if (SelectedAbility.index == 1)
        {
            // power up x attack
            player.GetComponent<PlayerAttack>().damage_x_multiplier = 1f + 0.1f * lv;
        }
        else if (SelectedAbility.index == 2)
        {
            // speed up z attack
            player.GetComponent<PlayerAttack>().Speed_Z = 1f + 0.2f * lv;
        }
        else if (SelectedAbility.index == 3)
        {
            // speed up z attack
            player.GetComponent<PlayerAttack>().Speed_X = 1f + 0.2f * lv;
        }
        else if (SelectedAbility.index == 5)
        {
            // run speed upgrade
            player.GetComponent<Player>().IncreaseRunSpeed();
        }
        else if (SelectedAbility.index == 6)
        {
            // 질풍 베기
            player.GetComponent<PlayerAttack>().sword_wind_enable = true;
        }
        else if (SelectedAbility.index == 7)
        {
            // 폭풍 베기
            player.GetComponent<PlayerAttack>().sword_storm_enable = true;
        }
        else if (SelectedAbility.index == 8)
        {
            // 흡혈
            player.GetComponent<PlayerAttack>().sword_cursed_enable = true;
        }
        else if (SelectedAbility.index == 9)
        {
            // 차징
            player.GetComponent<PlayerAttack>().sword_charging_enable = true;
        }
        else if (SelectedAbility.index == 10)
        {
            // 치명타
            player.GetComponent<PlayerAttack>().sword_critical_enable = true;
        }
        else if (SelectedAbility.index == 11)
        {
            // 뎀감
            player.GetComponent<PlayerAttack>().sword_shield_enable = true;
        }

        /*
        if (SelectedAbility.Type == 0)
        {
            this.gameObject.GetComponent<PlayerAttack>().Speed_Z *= 1.5f;
        }*/
        // *************
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            GameObject ability = abilityButtons[i];
            int type = ability.GetComponent<Ability>().Type;
            int tier = ability.GetComponent<Ability>().Tier;
            bool isSelected = ability.GetComponent<Ability>().isSelected;

            if (type == SelectedAbility.Type && !isSelected && lv == 1)  // 선택된 능력과 타입은 같으나 선택받지 못한 능력일 때
            {
                ability.GetComponent<Ability>().weight += 1;
                if (tier == SelectedAbility.Tier + 1)
                {
                    ability.GetComponent<Ability>().weight += 1;
                }
            }
            //if (abilityButtons[i].gameObject.GetComponent<Ability>().appeared) abilityButtons[i].gameObject.GetComponent<Ability>().appeared = false;
            ability.SetActive(false);
        }

    }
    public void UpgradeAbility()
    {
        // 선택된 어빌리티 인덱스 모음
        List<int> selected_list = new List<int>();

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            // 남아있는 어빌리티 초기화
            abilityButtons[i].GetComponent<Ability>().isAppeared = false;
            // 선택된 어빌리티 갯수 세기
            if (abilityButtons[i].GetComponent<Ability>().isSelected)
            {
                selected_list.Add(i);
            }
        }


        if (selected_list.Count == 0)
        {
            Debug.Log("There's no selected ability");
        }
        else
        {
            //
            Debug.Log("Selected ability is : " + selected_list.Count);
            Ability_UI.SetActive(true);
            int howmany = selected_list.Count;
            if (howmany > 3) howmany = 3;


            for (int j = 0; j < howmany; j++)
            {
                int rand = Random.Range(0, selected_list.Count);
                int target_index = selected_list[rand];

                // 강화할 어빌리티 출력
                abilityButtons[target_index].transform.position = buttonLocations[j].transform.position;
                abilityButtons[target_index].SetActive(true);
                abilityButtons[target_index].GetComponent<Ability>().isAppeared = true;

                // selected_list에서 뽑힌 것 제외하기
                selected_list.RemoveAt(rand);

            }
        }
    }

    public void GiveUp()
    {
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.ClearObjects();
        Destroy(GameManager.Instance.gameObject);
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
