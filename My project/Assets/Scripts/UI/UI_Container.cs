using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
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
    [SerializeField] private GameObject purchaseText;

    [SerializeField] private GameObject hashshin_gauge_UI;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image gaugeImage;
    float maxGauge = 100f;
    float curGauge;
    [SerializeField] private Color maxColor;

    // 어빌리티 UI
    [SerializeField] private GameObject Ability_UI;
    [SerializeField] private RectTransform[] buttonLocations;
    [SerializeField] private GameObject[] Ability_Array;
    [SerializeField] private List<GameObject> availableAbilityList;
    private List<GameObject> SelectedAbilityList = new List<GameObject>();

    [SerializeField] private GameObject[] hiddenKnightAbility_Z;
    [SerializeField] private GameObject[] hiddenKnightAbility_X;
    [SerializeField] private GameObject[] hiddenRangerAbility_Z;
    [SerializeField] private GameObject[] hiddenRangerAbility_X;
    [SerializeField] private GameObject[] hiddenHashashinAbility_Z;
    [SerializeField] private GameObject swiftAbility;

    /*[HideInInspector]*/ public List<int> SelectLog = new List<int>();

    private GameObject playerObject;
    private Player _player;
    private PlayerAttack _playerAttack;
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

    // 페이드 UI
    [SerializeField] private GameObject Fade_UI;

    // 옵션 UI
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider MasterAudioSlider;
    [SerializeField] private Slider BgmAudioSlider;
    [SerializeField] private Slider EffectAudioSlider;

    // 알람 UI
    [SerializeField] private GameObject AlermText;
    [SerializeField] private GameObject NoticeMainText;
    [SerializeField] private GameObject NoticeSubText;

    // 저장중 UI
    [SerializeField] private GameObject RotatingCircleUI;
    [HideInInspector] public bool saveDone;

    // UI 카운터
    public int popup_ui_counter = 0;

    // 플레이어 버프 UI
    [SerializeField] private GameObject PlayerBuffContainer;
    [HideInInspector] public List<PlayerBuff> playerBuffs = new List<PlayerBuff>();
    private float buff_space = 5f;

    // 플레이어 사망 UI
    [SerializeField] private GameObject PlayerDieUI;
    private void OnEnable()
    {
        Instance = this;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        _player = playerObject.GetComponent<Player>();
        _playerAttack = playerObject.GetComponent<PlayerAttack>();

        if (PlayerPrefs.GetInt("weaponType") == 3)
        {
            hashshin_gauge_UI.SetActive(true);
            curGauge = 0f;
            updateGauge(curGauge);
        }

        // 어빌리티 세팅
        for(int i = 0; i < Ability_Array.Length; i++)
        {
            Ability ability = Ability_Array[i].GetComponent<Ability>();
            ability.index = i;
            if (!ability.hidden) availableAbilityList.Add(Ability_Array[i]);
        }

        // 적 체력바 준비
        GrowEnemySliderPool();

        // 볼륨 슬라이더 세팅
        if (!PlayerPrefs.HasKey("MasterVolume")) PlayerPrefs.SetFloat("MasterVolume", 1f);
        MasterAudioSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        if (!PlayerPrefs.HasKey("BgmVolume")) PlayerPrefs.SetFloat("BgmVolume", 0.75f);
        BgmAudioSlider.value = PlayerPrefs.GetFloat("BgmVolume");
        if (!PlayerPrefs.HasKey("EffectVolume")) PlayerPrefs.SetFloat("EffectVolume", 0.75f);
        EffectAudioSlider.value = PlayerPrefs.GetFloat("EffectVolume");

        // 플레이어 세팅
        HandleGold(_player.CheckGold());
        HandleHP(_player.CurHP, _player.MaxHP);
    }
    private void Update()
    {
        UpdatePlayerBuffIcon();
    }
    public void Data_Recovery()
    {
        int[] select_log = DataManager.Instance.data.select_log;
        for(int i = 0; i < select_log.Length; i++)
        {
            SelectLog.Add(select_log[i]);
            GetAbility(Ability_Array[select_log[i]]);
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
        toggleText[idx].GetComponent<Animator>().SetBool("IsEnabled", true);
    }
    public void DisableToggleText(int idx)
    {
        toggleText[idx].GetComponent<Animator>().SetBool("IsEnabled", false);
    }
    public void EnablePurchaseText(int price)
    {
        purchaseText.GetComponent<Animator>().SetBool("IsEnabled", true);
        purchaseText.GetComponentInChildren<TextMeshProUGUI>().text = "Purchase " + price.ToString() + "G";
    }
    public void DisablePurchaseText()
    {
        purchaseText.GetComponent<Animator>().SetBool("IsEnabled", false);

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
    public void GetRandomAbility()
    {
        //if (Ability_UI.activeSelf) return;
        if (popup_ui_counter > 0 || PlayerDieUI.activeSelf) return;

        totalWeight = 0;
        // 새로운 리스트에 출현가능한 어빌리티들 복사
        List<GameObject> selection = new List<GameObject>();

        for(int i = 0; i < availableAbilityList.Count; i++)
        {
            totalWeight += availableAbilityList[i].GetComponent<Ability>().weight;
            availableAbilityList[i].SetActive(false);
            selection.Add(availableAbilityList[i]);
        }
        if (selection.Count == 0) AlermTextEnable("획득 가능한 어빌리티가 없습니다.");   // 남아있는 어빌리티가 없음
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

        if (ability.level < 10) ability.level++;
        SelectedAbility.SetActive(false);
        if (availableAbilityList.Contains(SelectedAbility))
        {
            // 처음 고른 어빌리티
            availableAbilityList.Remove(SelectedAbility);
            SelectedAbilityList.Add(SelectedAbility);
        }

        string name = SelectedAbility.name;
        if (name == "PowerUp_Z")
        {
            if (!_playerAttack.damage_z_buffer.ContainsKey("PowerUp_Z")) _playerAttack.damage_z_buffer.Add("PowerUp_Z", 0.2f * ability.level);
            else _playerAttack.damage_z_buffer["PowerUp_Z"] = 0.2f * ability.level;
            CollectionZ();
        }
        else if (name == "PowerUp_X")
        {
            if (!_playerAttack.damage_x_buffer.ContainsKey("PowerUp_X")) _playerAttack.damage_x_buffer.Add("PowerUp_X", 0.2f * ability.level);
            else _playerAttack.damage_x_buffer["PowerUp_X"] = 0.2f * ability.level;
            CollectionX();
        }
        else if (name == "SpeedUp_Z")
        {
            if (!_playerAttack.speed_z_buffer.ContainsKey("SpeedUp_Z")) _playerAttack.speed_z_buffer.Add("SpeedUp_Z", 0.2f * ability.level);
            else _playerAttack.speed_z_buffer["SpeedUp_Z"] = 0.2f * ability.level;
            CollectionZ();
        }
        else if (name == "SpeedUp_X")
        {
            if (!_playerAttack.speed_x_buffer.ContainsKey("SpeedUp_X")) _playerAttack.speed_x_buffer.Add("SpeedUp_X", 0.2f * ability.level);
            else _playerAttack.speed_x_buffer["SpeedUp_X"] = 0.2f * ability.level;
            CollectionX();
        }
        else if (name == "PowerUp")
        {
            if (!_playerAttack.power_buffer.ContainsKey("PowerUp")) _playerAttack.power_buffer.Add("PowerUp", 0.1f * ability.level);
            else _playerAttack.power_buffer["PowerUp"] = 0.1f * ability.level;
        }
        else if (name == "SpeedUp_Run")
        {
            _player.moveSpeed_multiplier = 1f + 0.1f * ability.level;
            if (ability.level == 1) UnlockSwift();
        }
        else if (name == "DefenceUp")
        {
            _player.defence_multiplier = 1f + 0.1f * ability.level;
        }
        else if (name == "Dodge")
        {
            _player.dodge_enable = true;
        }
        else if (name == "PotionMaster")
        {
            _player.hpincrease_multiplier = 1.3f;
        }
        else if (name == "Recovery")
        {
            _player.recovery_enable = true;
        }
        else if (name == "Guard")
        {
            _player.guard_enable = true;
        }
        else if (name == "Aura")
        {
            ///////////////////////////////
        }
        else if (name == "GoldRush")
        {
            _player.gold_multiplier = 1.2f;
        }
        else if (name == "Resistance")
        {
            _player.resistance_enable = true;
        }
        else if (name == "SwordWind")
        {
            _playerAttack.sword_wind_enable = true;
        }
        else if (name == "StormSlash")
        {
            _playerAttack.sword_storm_enable = true;
        }
        else if (name == "CursedSlash")
        {
            _playerAttack.sword_cursed_enable = true;
        }
        else if (name == "ChargeSlash")
        {
            _playerAttack.sword_charging_enable = true;
        }
        else if (name == "CriticalSlash")
        {
            _playerAttack.sword_critical_enable = true;
        }
        else if (name == "DefenceSlash")
        {
            _playerAttack.sword_shield_enable = true;
        }
        else if (name == "StormShot")
        {
            _playerAttack.bow_storm_enable = true;
        }
        else if (name == "PoisonShot")
        {
            _playerAttack.bow_poison_enable = true;
        }
        else if (name == "AirShot")
        {
            _playerAttack.bow_air_enable = true;
        }
        else if (name == "ArrowRain")
        {
            _playerAttack.bow_rain_enable = true;
        }
        else if (name == "HardPlant")
        {
            _playerAttack.bow_slow_enable = true;
        }
        else if (name == "QuickShower")
        {
            _playerAttack.bow_fast_enable = true;
        }
        else if (name == "DaggerStorm")
        {
            _playerAttack.dagger_storm_enable = true;
            availableAbilityList.Remove(hiddenHashashinAbility_Z[1]);
            availableAbilityList.Remove(hiddenHashashinAbility_Z[2]);
        }
        else if (name == "QuickWind")
        {
            _playerAttack.quick_wind_enable = true;
            availableAbilityList.Remove(hiddenHashashinAbility_Z[0]);
        }
        else if (name == "Assassin")
        {
            _playerAttack.assassin_enable = true;
            availableAbilityList.Remove(hiddenHashashinAbility_Z[0]);
            _playerAttack.speed_x_buffer.Add("Assassin", 0.5f);
            _playerAttack.damage_x_buffer.Add("Assassin", 0.5f);
        }
        else if (name == "Swift")
        {
            _playerAttack.swift_enable = true;
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
    public void SelectLogUpdate(Ability ability)
    {
        SelectLog.Add(ability.index);
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
    public void UpgradeRandomAbility()
    {
        //if (Ability_UI.activeSelf) return;
        if (popup_ui_counter > 0 || PlayerDieUI.activeSelf) return;

        List<GameObject> selection = new List<GameObject>();
        for(int i = 0; i < SelectedAbilityList.Count; i++)
        {
            SelectedAbilityList[i].SetActive(false);
            // 레벨 10 미만 어빌리티들만 selection 리스트에 추가. 몇몇 어빌리티들은 이미 11레벨로 설정.
            if (SelectedAbilityList[i].GetComponent<Ability>().level < 10) selection.Add(SelectedAbilityList[i]);
        }

        if (selection.Count == 0) AlermTextEnable("강화가능한 어빌리티가 없습니다.");
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
        Time.timeScale = 1f;
        StartCoroutine(GameManager.Instance.GiveUpFlow());
    }
    public void QuitGame()
    {
        Time.timeScale = 1f;
        /*int length = SelectLog.Count;
        int[] arr = new int[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = SelectLog[i];
        }
        DataManager.Instance.data.select_log = arr;*/

        StartCoroutine(GameManager.Instance.QuitGameFlow());
    }
    public void SaveSelectLog()
    {
        int[] arr = new int[SelectLog.Count];
        for (int i = 0; i < arr.Length; i++) arr[i] = SelectLog[i];
        DataManager.Instance.data.select_log = arr;
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
        instance.SetActive(false);
        enemySliderQueue.Enqueue(instance);
    }
    public GameObject GetFromEnemySliderPool()
    {
        if (enemySliderQueue.Count == 0) GrowEnemySliderPool();
        var instance = enemySliderQueue.Dequeue();
        return instance;
    }
    public IEnumerator FadeOutStart()
    {
        Fade_UI.SetActive(true);
        GameManager.Instance.fadeState = "fading";
        Image fadeImage = Fade_UI.GetComponent<Image>();
        for(float f = 0f; f < 1f; f += Time.deltaTime * 2)
        {
            Color c = fadeImage.color;
            c.a = f;
            fadeImage.color = c;
            yield return null;
        }
        GameManager.Instance.fadeState = "faded";
        yield break;
    }
    public IEnumerator FadeInStart()
    {
        Fade_UI.SetActive(true);
        GameManager.Instance.fadeState = "fading";
        Image fadeImage = Fade_UI.GetComponent<Image>();

        Color temp = fadeImage.color;
        temp.a = 1f;
        fadeImage.color = temp;
        yield return new WaitForSecondsRealtime(0.5f);

        for (float f = 1f; f > 0f; f -= Time.deltaTime * 2)
        {
            Color c = fadeImage.color;
            c.a = f;
            fadeImage.color = c;
            yield return null;
        }
        Fade_UI.SetActive(false);
        GameManager.Instance.fadeState = "clear";
        yield break;
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
    public void AlermTextEnable(string text)
    {
        AlermText.GetComponent<TextMeshProUGUI>().text = text;
        AlermText.GetComponent<Animator>().ResetTrigger("AlermTrigger");
        AlermText.GetComponent<Animator>().SetTrigger("AlermTrigger");
    }
    public void NoticeMainTextEnable(string text)
    {
        NoticeMainText.GetComponent<TextMeshProUGUI>().text = text;
        NoticeMainText.GetComponent<Animator>().ResetTrigger("AlermTrigger");
        NoticeMainText.GetComponent<Animator>().SetTrigger("AlermTrigger");
    }
    public void NoticeSubTextEnable(string text)
    {
        NoticeSubText.GetComponent<TextMeshProUGUI>().text = text;
    }
    public IEnumerator StartSaving()
    {
        GameObject textObj = RotatingCircleUI.transform.GetChild(0).gameObject;
        GameObject imageObj = RotatingCircleUI.transform.GetChild(1).gameObject;
        if (textObj == null || imageObj == null) yield break;
        textObj.SetActive(true);
        imageObj.SetActive(true);
        TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
        Image image = imageObj.GetComponent<Image>();
        saveDone = false;
        yield return new WaitUntil(() => saveDone);

        for (float _alpha = 1f; _alpha > 0f; _alpha -= Time.unscaledDeltaTime * 0.4f)
        {
            Color c = image.color;
            c.a = _alpha;
            image.color = c;
            text.color = c;
            yield return null;
        }
        textObj.SetActive(false);
        imageObj.SetActive(false);
    }
    public void PopUpControl()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Esc_UI.SetActive(true);
        }
        // esc ui가 켜져있지 않으면 book ui 활성화
        else if (Input.GetKeyDown(KeyCode.B) && !Esc_UI.activeSelf)
        {
            Book_UI.SetActive(true);
        }
    }
    public void AddPlayerBuff(string buff_name, float max_dur)
    {
        PlayerBuff buff = new PlayerBuff
        {
            name = buff_name,
            max_duration = max_dur,
            cur_duration = max_dur
        };
        bool exist = false;
        for(int i = 0; i < playerBuffs.Count; i++)
        {
            if (playerBuffs[i].name == buff.name)
            {
                exist = true;
                playerBuffs[i] = buff;
                break;
            }
        }
        if (!exist) playerBuffs.Add(buff);
    }
    private void UpdatePlayerBuffIcon()
    {
        float x = 0f;
        for(int i = 0; i < playerBuffs.Count; i++)
        {
            PlayerBuff buff = playerBuffs[i];

            GameObject buffIcon = PlayerBuffContainer.transform.Find(buff.name).gameObject;
            if (buffIcon != null)
            {
                Image iconImage = buffIcon.GetComponent<Image>();
                if (buff.cur_duration > 0f)
                {
                    if (buff.name == "Dodge")
                    {
                        _playerAttack.dodgeBuff = true;
                    }
                    //
                    buff.cur_duration -= Time.deltaTime;
                    buffIcon.SetActive(true);
                    RectTransform rectTransform = buffIcon.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(x, 0f);

                    iconImage.fillAmount = buff.cur_duration / buff.max_duration;
                    x -= rectTransform.sizeDelta.x + buff_space;
                }
                else if (buff.max_duration == -1f)
                {
                    buffIcon.SetActive(true);
                    RectTransform rectTransform = buffIcon.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(x, 0f);

                    iconImage.fillAmount = 1f;
                    x -= rectTransform.sizeDelta.x + buff_space;
                }
                else
                {
                    // 버프 소진시
                    if (buff.name == "Dodge")
                    {
                        _playerAttack.dodgeBuff = false;
                    }
                    buffIcon.SetActive(false);
                }
            }
        }
    }
    public void EnableDieUI()
    {
        PlayerDieUI.SetActive(true);
    }
}
