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

    // �����Ƽ UI
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

    [HideInInspector] public List<int> SelectLog = new List<int>();

    private GameObject playerObject;
    private int z_collection = 0;
    private int x_collection = 0;
    int totalWeight;

    [SerializeField] private GameObject Esc_UI;

    // ȹ�� �����Ƽ UI
    [SerializeField] private GameObject Book_UI;
    private List<GameObject> ScrollViewList = new List<GameObject>();
    [SerializeField] private ScrollRect scrollRect;
    float space = 50f;

    // �� ü�¹� UI
    [SerializeField] private GameObject Enemy_UI;
    [SerializeField] private GameObject enemyHpSlider;
    private Queue<GameObject> enemySliderQueue = new Queue<GameObject>();

    // ���̵� UI
    [SerializeField] private GameObject Fade_UI;
    public bool fade_in_start;
    private bool faded;

    // �ɼ� UI
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider MasterAudioSlider;
    [SerializeField] private Slider BgmAudioSlider;
    [SerializeField] private Slider EffectAudioSlider;
    private void OnEnable()
    {
        Instance = this;
        if (PlayerPrefs.GetInt("weaponType") == 3)
        {
            hashshin_gauge_UI.SetActive(true);
            curGauge = 0f;
            updateGauge(curGauge);
        }

        // �����Ƽ ����
        for(int i = 0; i < Ability_Array.Length; i++)
        {
            Ability ability = Ability_Array[i].GetComponent<Ability>();
            ability.index = i;
            if (!ability.hidden) availableAbilityList.Add(Ability_Array[i]);
        }

        // �� ü�¹� �غ�
        GrowEnemySliderPool();

        // ���� �����̴� ����
        if (!PlayerPrefs.HasKey("MasterVolume")) PlayerPrefs.SetFloat("MasterVolume", 1f);
        MasterAudioSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        if (!PlayerPrefs.HasKey("BgmVolume")) PlayerPrefs.SetFloat("BgmVolume", 0.75f);
        BgmAudioSlider.value = PlayerPrefs.GetFloat("BgmVolume");
        if (!PlayerPrefs.HasKey("EffectVolume")) PlayerPrefs.SetFloat("EffectVolume", 0.75f);
        EffectAudioSlider.value = PlayerPrefs.GetFloat("EffectVolume");

        faded = false;
    }
    private void Update()
    {
        if (!faded)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Esc_UI.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                Book_UI.SetActive(true);
            }
        }
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
        //Debug.Log("Enable toggle text : " + idx);
        toggleText[idx].GetComponent<Animator>().SetBool("IsEnabled", true);
    }
    public void DisableToggleText(int idx)
    {
        //Debug.Log("Disable toggle text : " + idx);
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
    public void RandomAbility()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");

        totalWeight = 0;
        // ���ο� ����Ʈ�� ���������� �����Ƽ�� ����
        List<GameObject> selection = new List<GameObject>();

        for(int i = 0; i < availableAbilityList.Count; i++)
        {
            //availableAbilityList[i].GetComponent<Ability>().isAppeared = false;
            totalWeight += availableAbilityList[i].GetComponent<Ability>().weight;
            availableAbilityList[i].SetActive(false);
            selection.Add(availableAbilityList[i]);
        }
        if (selection.Count == 0) Debug.Log("There's no remain ability");   // �����ִ� �����Ƽ�� ����
        else
        {
            Ability_UI.SetActive(true);

            int idx = 0;
            int rand, weight;
            while (selection.Count > 0)
            {
                rand = Random.Range(1, totalWeight + 1);    // 1~totalweight ���� ����
                weight = 0;
                // ����ġ ��� selection ����Ʈ���� �ϳ� �̱�
                for(int i = 0; i < selection.Count; i++)
                {
                    weight += selection[i].GetComponent<Ability>().weight;
                    if (rand <= weight)
                    {
                        // �ش� �����Ƽ �̱�
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
                // 3�� �̾����� break
                if (idx == 3) break;
            }
        }
    }
    public void GetAbility(GameObject SelectedAbility)
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");

        Ability ability = SelectedAbility.GetComponent<Ability>();
        Player player = playerObject.GetComponent<Player>();
        PlayerAttack playerAttack = playerObject.GetComponent<PlayerAttack>();

        if (ability.level < 10) ability.level++;
        SelectedAbility.SetActive(false);
        if (availableAbilityList.Contains(SelectedAbility))
        {
            // ó�� �� �����Ƽ
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
            CollectionZ();
        }
        else if (name == "PowerUp_X")
        {
            //playerAttack.damage_x_multiplier = 1f + 0.1f * ability.level;
            if (!playerAttack.damage_x_buffer.ContainsKey("PowerUp_X")) playerAttack.damage_x_buffer.Add("PowerUp_X", 0.2f * ability.level);
            else playerAttack.damage_x_buffer["PowerUp_X"] = 0.2f * ability.level;
            CollectionX();
        }
        else if (name == "SpeedUp_Z")
        {
            //playerAttack.Speed_Z = 1f + 0.2f * ability.level;
            if (!playerAttack.speed_z_buffer.ContainsKey("SpeedUp_Z")) playerAttack.speed_z_buffer.Add("SpeedUp_Z", 0.2f * ability.level);
            else playerAttack.speed_z_buffer["SpeedUp_Z"] = 0.2f * ability.level;
            CollectionZ();
        }
        else if (name == "SpeedUp_X")
        {
            //playerAttack.Speed_X = 1f + 0.2f * ability.level;
            if (!playerAttack.speed_x_buffer.ContainsKey("SpeedUp_X")) playerAttack.speed_x_buffer.Add("SpeedUp_X", 0.2f * ability.level);
            else playerAttack.speed_x_buffer["SpeedUp_X"] = 0.2f * ability.level;
            CollectionX();
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
            if (ability.level == 1) UnlockSwift();
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
                // ���� ���� �����Ƽ ����
                for (int i = 0; i < hiddenKnightAbility_Z.Length; i++)
                {
                    availableAbilityList.Add(hiddenKnightAbility_Z[i]);
                }
            }
            else if (weaponType == 2)
            {
                // �ü� ���� �����Ƽ ����
                for (int i = 0; i < hiddenRangerAbility_Z.Length; i++)
                {
                    availableAbilityList.Add(hiddenRangerAbility_Z[i]);
                }
            }
            else if (weaponType == 3)
            {
                // ���� ���� �����Ƽ ����
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
                // ���� ���� �����Ƽ ����
                for (int i = 0; i < hiddenKnightAbility_X.Length; i++)
                {
                    availableAbilityList.Add(hiddenKnightAbility_X[i]);
                }
            }
            else if (weaponType == 2)
            {
                // �ü� ���� �����Ƽ ����
                for (int i = 0; i < hiddenRangerAbility_X.Length; i++)
                {
                    availableAbilityList.Add(hiddenRangerAbility_X[i]);
                }
            }
        }
    }
    public void UnlockSwift()
    {
        // ������Ʈ�� ������ �߰�
        if (!availableAbilityList.Contains(swiftAbility)) availableAbilityList.Add(swiftAbility);
    }
    public void UpgradeAbility()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");

        List<GameObject> selection = new List<GameObject>();
        for(int i = 0; i < SelectedAbilityList.Count; i++)
        {
            SelectedAbilityList[i].SetActive(false);
            // ���� 10 �̸� �����Ƽ�鸸 selection ����Ʈ�� �߰�. ��� �����Ƽ���� �̹� 11������ ����.
            if (SelectedAbilityList[i].GetComponent<Ability>().level < 10) selection.Add(SelectedAbilityList[i]);
        }

        if (selection.Count == 0) Debug.Log("��ȭ������ �����Ƽ�� �����ϴ�.");
        else
        {
            Debug.Log("��ȭ������ �����Ƽ ���� : " + selection.Count);
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
                // 3�� �̾����� break
                if (idx == 3) break;
            }
        }
    }

    public void GiveUp()
    {
        StartCoroutine(GameManager.Instance.GiveUpFlow());
    }
    public void QuitGame()
    {
        int length = SelectLog.Count;
        int[] arr = new int[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = SelectLog[i];
        }
        DataManager.Instance.data.select_log = arr;

        StartCoroutine(GameManager.Instance.QuitGameFlow());
    }
    public void AddToBook(GameObject gameObject)
    {
        Ability selectedAbility = gameObject.GetComponent<Ability>();
        if (selectedAbility.level == 1 || selectedAbility.level == 11)
        {
            // ù ȹ���� �����Ƽ
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
    public IEnumerator FadeFlow()
    {
        Fade_UI.SetActive(true);
        faded = true;
        for (float f = 0f; f < 1f; f += Time.deltaTime * 2)
        {
            Color c = Fade_UI.GetComponent<Image>().color;
            c.a = f;
            Fade_UI.GetComponent<Image>().color = c;
            yield return null;
        }

        GameManager.Instance.faded = true;
        fade_in_start = false;
        yield return new WaitUntil(() => fade_in_start);
        yield return new WaitForSecondsRealtime(0.5f);
        Debug.Log("���̵��� ����");
        for (float f = 1f; f > 0f; f -= Time.deltaTime * 2)
        {
            Color c = Fade_UI.GetComponent<Image>().color;
            c.a = f;
            Fade_UI.GetComponent<Image>().color = c;
            yield return null;
        }
        //yield return new WaitForSeconds(0.1f);
        Fade_UI.SetActive(false);
        faded = false;
        GameManager.Instance.faded = false;
        Debug.Log("���̵� ��");
        yield break;
    }
    public IEnumerator FadeOutStart()
    {
        Fade_UI.SetActive(true);
        Image fadeImage = Fade_UI.GetComponent<Image>();
        for(float f = 0f; f < 1f; f += Time.deltaTime * 2)
        {
            Color c = fadeImage.color;
            c.a = f;
            fadeImage.color = c;
            yield return null;
        }
        GameManager.Instance.faded = true;
        yield break;
    }
    public IEnumerator FadeInStart()
    {
        Debug.Log("���̵��� ����");
        Fade_UI.SetActive(true);
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
        GameManager.Instance.faded = false;
        Debug.Log("���̵� ��");
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
}
