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
    float MaxHP;
    float CurHP;

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
    [SerializeField] private List<GameObject> availableAbilityList;
    private List<GameObject> SelectedAbilityList = new List<GameObject>();
    [SerializeField] private GameObject[] hiddenKnightAbility_Z;
    [SerializeField] private GameObject[] hiddenKnightAbility_X;
    [SerializeField] private GameObject[] hiddenRangerAbility_Z;
    [SerializeField] private GameObject[] hiddenRangerAbility_X;
    [SerializeField] private GameObject[] hiddenHashashinAbility_Z;
    [SerializeField] private GameObject swiftAbility;

    private GameObject playerObject;
    private int z_collection = 0;
    private int x_collection = 0;
    int totalWeight;

    [SerializeField] private GameObject Esc_UI;

    // 획득 어빌리티 UI
    [SerializeField] private GameObject Book_UI;
    private List<GameObject> ScrollViewList = new List<GameObject>();
    [SerializeField] private ScrollRect scrollRect;
    float space = 50f;

    // 적 체력바 UI
    [SerializeField] private GameObject Enemy_UI;
    [SerializeField] private GameObject enemyHpSlider;
    private Queue<GameObject> enemySliderQueue = new Queue<GameObject>();
    private void OnEnable()
    {
        Instance = this;
        if (PlayerPrefs.GetInt("weaponType") == 3) hashshin_gauge_UI.SetActive(true);
        curGauge = 0f;

        // 적 체력바 준비
        GrowEnemySliderPool();
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
        playerObject = GameObject.FindGameObjectWithTag("Player");

        totalWeight = 0;
        // 새로운 리스트에 출현가능한 어빌리티들 복사
        List<GameObject> selection = new List<GameObject>();

        for(int i = 0; i < availableAbilityList.Count; i++)
        {
            //availableAbilityList[i].GetComponent<Ability>().isAppeared = false;
            totalWeight += availableAbilityList[i].GetComponent<Ability>().weight;
            availableAbilityList[i].SetActive(false);
            selection.Add(availableAbilityList[i]);
        }
        if (selection.Count == 0) Debug.Log("There's no remain ability");   // 남아있는 어빌리티가 없음
        else
        {
            Ability_UI.SetActive(true);

            int idx = 0;
            int rand, weight;
            while (selection.Count > 0)
            {
                rand = Random.Range(1, totalWeight + 1);    // 1~totalweight 까지 랜덤
                weight = 0;
                // 가중치 기반 selection 리스트에서 하나 뽑기
                for(int i = 0; i < selection.Count; i++)
                {
                    weight += selection[i].GetComponent<Ability>().weight;
                    if (rand <= weight)
                    {
                        // 해당 어빌리티 뽑기
                        rand = i;
                        break;
                    }
                }
                RectTransform rect = selection[rand].GetComponent<RectTransform>();
                rect.anchoredPosition = buttonLocations[idx].anchoredPosition;
                selection[rand].SetActive(true);
                totalWeight -= selection[rand].GetComponent<Ability>().weight;
                selection.RemoveAt(rand);
                idx++;
                // 3개 뽑았으면 break
                if (idx == 3) break;
            }
        }
    }
    public void GetAbility(GameObject SelectedAbility)
    {
        Ability ability = SelectedAbility.GetComponent<Ability>();
        Player player = playerObject.GetComponent<Player>();
        PlayerAttack playerAttack = playerObject.GetComponent<PlayerAttack>();

        if (ability.level < 10) ability.level++;
        SelectedAbility.SetActive(false);
        if (availableAbilityList.Contains(SelectedAbility))
        {
            // 처음 고른 어빌리티
            availableAbilityList.Remove(SelectedAbility);
            SelectedAbilityList.Add(SelectedAbility);
        }

        string name = SelectedAbility.name;
        Debug.Log("Selected ability is : " + name);
        if (name == "PowerUp_Z")
        {
            //playerAttack.damage_z_multiplier = 1f + 0.1f * ability.level;
            if (!playerAttack.damage_z_buffer.ContainsKey("PowerUp_Z")) playerAttack.damage_z_buffer.Add("PowerUp_Z", 0.2f * ability.level);
            else playerAttack.damage_z_buffer["PowerUp_Z"] = 0.2f * ability.level;

        }
        else if (name == "PowerUp_X")
        {
            //playerAttack.damage_x_multiplier = 1f + 0.1f * ability.level;
            if (!playerAttack.damage_x_buffer.ContainsKey("PowerUp_X")) playerAttack.damage_x_buffer.Add("PowerUp_X", 0.2f * ability.level);
            else playerAttack.damage_x_buffer["PowerUp_X"] = 0.2f * ability.level;

        }
        else if (name == "SpeedUp_Z")
        {
            //playerAttack.Speed_Z = 1f + 0.2f * ability.level;
            if (!playerAttack.speed_z_buffer.ContainsKey("SpeedUp_Z")) playerAttack.speed_z_buffer.Add("SpeedUp_Z", 0.2f * ability.level);
            else playerAttack.speed_z_buffer["SpeedUp_Z"] = 0.2f * ability.level;
        }
        else if (name == "SpeedUp_X")
        {
            //playerAttack.Speed_X = 1f + 0.2f * ability.level;
            if (!playerAttack.speed_x_buffer.ContainsKey("SpeedUp_X")) playerAttack.speed_x_buffer.Add("SpeedUp_X", 0.2f * ability.level);
            else playerAttack.speed_x_buffer["SpeedUp_X"] = 0.2f * ability.level;

        }
        else if (name == "PowerUp")
        {
            //playerAttack.playerPower = 100f * (1f + 0.1f * ability.level);
            if (!playerAttack.power_buffer.ContainsKey("PowerUp")) playerAttack.power_buffer.Add("PowerUp", 0.1f * ability.level);
            else playerAttack.power_buffer["PowerUp"] = 0.1f * ability.level;
        }
        else if (name == "SpeedUp_Run")
        {
            player.moveSpeed_multiplier = 1f + 0.1f * ability.level;
        }
        else if (name == "DefenceUp")
        {
            player.defence_multiplier = 1f + 0.1f * ability.level;
        }
        else if (name == "Dodge")
        {

        }
        else if (name == "SecondHeart")
        {
            player.hpincrease_multiplier = 1.3f;
        }
        else if (name == "Recovery")
        {
            player.recovery_enable = true;
        }
        else if (name == "Guard")
        {

        }
        else if (name == "Aura")
        {

        }
        else if (name == "GoldRush")
        {
            player.gold_multiplier = 1.2f;
        }
        else if (name == "Resistance")
        {
            player.resistance_enable = true;
        }
        else if (name == "SwordWind")
        {
            playerAttack.sword_wind_enable = true;
        }
        else if (name == "StormSlash")
        {
            playerAttack.sword_storm_enable = true;
        }
        else if (name == "CursedSlash")
        {
            playerAttack.sword_cursed_enable = true;
        }
        else if (name == "ChargeSlash")
        {
            playerAttack.sword_charging_enable = true;
        }
        else if (name == "CriticalSlash")
        {
            playerAttack.sword_critical_enable = true;
        }
        else if (name == "DefenceSlash")
        {
            playerAttack.sword_shield_enable = true;
        }
        else if (name == "StormShot")
        {
            playerAttack.bow_storm_enable = true;
        }
        else if (name == "PoisonShot")
        {
            playerAttack.bow_poison_enable = true;
        }
        else if (name == "AirShot")
        {
            playerAttack.bow_air_enable = true;
        }
        else if (name == "ArrowRain")
        {
            playerAttack.bow_rain_enable = true;
        }
        else if (name == "HardPlant")
        {
            playerAttack.bow_slow_enable = true;
        }
        else if (name == "QuickShower")
        {
            playerAttack.bow_fast_enable = true;
        }
        else if (name == "DaggerStorm")
        {
            playerAttack.dagger_storm_enable = true;
            availableAbilityList.Remove(hiddenHashashinAbility_Z[1]);
            availableAbilityList.Remove(hiddenHashashinAbility_Z[2]);
        }
        else if (name == "QuickWind")
        {
            playerAttack.quick_wind_enable = true;
            availableAbilityList.Remove(hiddenHashashinAbility_Z[0]);
        }
        else if (name == "Assassin")
        {
            playerAttack.assassin_enable = true;
            availableAbilityList.Remove(hiddenHashashinAbility_Z[0]);
            playerAttack.speed_x_buffer.Add("Assassin", 0.5f);
            playerAttack.damage_x_buffer.Add("Assassin", 0.5f);
        }
        else if (name == "Swift")
        {

        }
        AddToBook(SelectedAbility);
        for (int i = 0; i < availableAbilityList.Count; i++)
        {
            availableAbilityList[i].SetActive(false);
        }
        for (int i = 0; i < SelectedAbilityList.Count; i++)
        {
            SelectedAbilityList[i].SetActive(false);
        }
    }
    public void CollectionZ()
    {
        z_collection++;
        if (z_collection == 2)
        {
            int weaponType = PlayerPrefs.GetInt("weaponType");
            if (weaponType == 1)
            {
                // 전사 히든 어빌리티 개방
                for (int i = 0; i < hiddenKnightAbility_Z.Length; i++)
                {
                    availableAbilityList.Add(hiddenKnightAbility_Z[i]);
                }
            }
            else if (weaponType == 2)
            {
                // 궁수 히든 어빌리티 개방
                for (int i = 0; i < hiddenRangerAbility_Z.Length; i++)
                {
                    availableAbilityList.Add(hiddenRangerAbility_Z[i]);
                }
            }
            else if (weaponType == 3)
            {
                // 도적 히든 어빌리티 개방
                for (int i = 0; i < hiddenHashashinAbility_Z.Length; i++)
                {
                    availableAbilityList.Add(hiddenHashashinAbility_Z[i]);
                }
            }
        }
    }
    public void CollectionX()
    {
        x_collection++;
        if (x_collection == 2)
        {
            int weaponType = PlayerPrefs.GetInt("weaponType");
            if (weaponType == 1)
            {
                // 전사 히든 어빌리티 개방
                for (int i = 0; i < hiddenKnightAbility_X.Length; i++)
                {
                    availableAbilityList.Add(hiddenKnightAbility_X[i]);
                }
            }
            else if (weaponType == 2)
            {
                // 궁수 히든 어빌리티 개방
                for (int i = 0; i < hiddenRangerAbility_X.Length; i++)
                {
                    availableAbilityList.Add(hiddenRangerAbility_X[i]);
                }
            }
        }
    }
    public void UnlockSwift()
    {
        // 스위프트가 없으면 추가
        if (!availableAbilityList.Contains(swiftAbility)) availableAbilityList.Add(swiftAbility);
    }
    public void UpgradeAbility()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");

        List<GameObject> selection = new List<GameObject>();
        for(int i = 0; i < SelectedAbilityList.Count; i++)
        {
            SelectedAbilityList[i].SetActive(false);
            // 레벨 10 미만 어빌리티들만 selection 리스트에 추가. 몇몇 어빌리티들은 이미 11레벨로 설정.
            if (SelectedAbilityList[i].GetComponent<Ability>().level < 10) selection.Add(SelectedAbilityList[i]);
        }

        if (selection.Count == 0) Debug.Log("강화가능한 어빌리티가 없습니다.");
        else
        {
            Debug.Log("강화가능한 어빌리티 개수 : " + selection.Count);
            Ability_UI.SetActive(true);
            int idx = 0;
            int rand;
            while (selection.Count > 0)
            {
                rand = Random.Range(0, selection.Count);

                RectTransform rect = selection[rand].GetComponent<RectTransform>();
                rect.anchoredPosition = buttonLocations[idx].anchoredPosition;
                selection[rand].SetActive(true);

                selection.RemoveAt(rand);
                idx++;
                // 3개 뽑았으면 break
                if (idx == 3) break;
            }
        }
    }

    public void GiveUp()
    {
        GameManager.Instance.ClearObjects();
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        DataManager.Instance.SaveGameData();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    public void AddToBook(GameObject gameObject)
    {
        Ability selectedAbility = gameObject.GetComponent<Ability>();
        if (selectedAbility.level == 1 || selectedAbility.level == 11)
        {
            // 첫 획득한 어빌리티
            var newUi = Instantiate(gameObject, scrollRect.content);
            newUi.SetActive(true);
            newUi.name = gameObject.name;
            newUi.GetComponent<Button>().enabled = false;
            ScrollViewList.Add(newUi);
        }
        else
        {
            for(int i = 0; i < ScrollViewList.Count; i++)
            {
                if (ScrollViewList[i].name == gameObject.name)
                {
                    ScrollViewList[i].GetComponent<Ability>().level = selectedAbility.level;
                    break;
                }
            }
        }
        UpdateBook();
    }
    private void UpdateBook()
    {
        float y = 0f;
        for(int i = 0; i < ScrollViewList.Count; i++)
        {
            RectTransform rectTransform = ScrollViewList[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, -y);
            y += rectTransform.sizeDelta.y + space;
        }
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
    }

    private void GrowEnemySliderPool()
    {
        for (int i = 0; i < 5; i++)
        {
            var instanceToAdd = Instantiate(enemyHpSlider);
            instanceToAdd.transform.SetParent(Enemy_UI.transform);
            AddToEnemySliderPool(instanceToAdd);
        }
    }
    public void AddToEnemySliderPool(GameObject instance)
    {
        Debug.Log("Hello");
        instance.SetActive(false);
        enemySliderQueue.Enqueue(instance);
    }
    public GameObject GetFromEnemySliderPool()
    {
        if (enemySliderQueue.Count == 0) GrowEnemySliderPool();
        var instance = enemySliderQueue.Dequeue();
        return instance;
    }
}
