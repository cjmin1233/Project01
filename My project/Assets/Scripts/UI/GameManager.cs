using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameManager() { }
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<GameManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = new GameObject().AddComponent<GameManager>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }
    private List<GameObject> objects;
    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject startCamera;
    [SerializeField] private GameObject PlayerAfterImagePool;
    [SerializeField] private GameObject DamageTextPool;
    [SerializeField] private GameObject HitFxPool;
    [SerializeField] private GameObject EnemyBulletPool;
    [SerializeField] private GameObject SwordWindPool;
    [SerializeField] private GameObject ArrowPool;
    [SerializeField] private GameObject ArrowShowerPool;
    [SerializeField] private GameObject EnemyPoolPrefab;
    [SerializeField] private GameObject ui_container;
    [SerializeField] private GameObject eventSystem;
    private int weaponType;
    private Data gameData;
    private GameObject player;
    private GameObject boss;
    private GameObject MainCamera;
    private GameObject Camera_Background_Container;
    private Vector3 tempPos;
    [SerializeField] private float camSpeed;
    [SerializeField] private Vector3 difValue;

    [HideInInspector] public bool newGame;
    [HideInInspector] public bool playerFollowing;
    [HideInInspector] public bool bossFollowing;
    [HideInInspector] public bool isPlaying;
    [HideInInspector] public int loadingType;

    // ���̵��� ȿ��, �ε�ȭ��
    [HideInInspector] public string gameState;
    [HideInInspector] public bool faded;
    [HideInInspector] public string fadeState;
    private Vector3 destination;

    // ����Ʈ ��ũ��
    [SerializeField] private GameObject SerpentScrewPrefab;
    private void Awake()
    {
        #region ��ü ����ȭ
        var objs = FindObjectsOfType<GameManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        #endregion

        #region �ʱ� ����
        objects = new List<GameObject>();

        playerFollowing = true;
        bossFollowing = false;
        isPlaying = false;
        loadingType = 0;

        var event_system = Instantiate(eventSystem);
        event_system.name = eventSystem.name;
        DontDestroyOnLoad(event_system);

        var cam_obj = Instantiate(startCamera);
        MainCamera = cam_obj;
        MainCamera.name = startCamera.name;
        DontDestroyOnLoad(MainCamera);
        Camera_Background_Container = MainCamera.transform.GetChild(0).gameObject;
        #endregion
    }
    private void Update()
    {
        if (isPlaying)
        {
            if (fadeState == "clear")
            {
                // ���̵� ���� ȭ���� ��
                UI_Container.Instance.PopUpControl();
                if (UI_Container.Instance.popup_ui_counter > 0) Time.timeScale = 0f;
                else if (bossFollowing) Time.timeScale = 0.3f;
                else if (playerFollowing) Time.timeScale = 1f;
                else Time.timeScale = 1f;

                // cheat key
                if (Input.GetKeyDown(KeyCode.C)) StartCoroutine(TransportFlow(Vector3.zero, true));
            }
        }
    }
    private void FixedUpdate()
    {
        if (isPlaying)
        {
            float _size = MainCamera.GetComponent<Camera>().orthographicSize;
            //int popup_counter = UI_Container.Instance.popup_ui_counter;
            if (bossFollowing)
            {
                MainCamera.GetComponent<Camera>().orthographicSize = _size > 3f ? _size - Time.deltaTime * 2f : 3f;
                if (boss == null) boss = GameObject.FindGameObjectWithTag("Boss");
                tempPos = Vector3.Lerp(MainCamera.transform.position, boss.transform.position + difValue, camSpeed);
                MainCamera.transform.position = new Vector3(tempPos.x, tempPos.y, -10f);
            }
            else if (playerFollowing)
            {
                // �÷��̾� ���� ī�޶�
                MainCamera.GetComponent<Camera>().orthographicSize = _size < 5f ? _size + Time.deltaTime * 2f : 5f;
                tempPos = Vector3.Lerp(MainCamera.transform.position, player.transform.position + difValue, camSpeed);
                MainCamera.transform.position = new Vector3(tempPos.x, tempPos.y, -10f);
            }
        }
    }
    public void PlayGame(int selected)
    {
        // ���θ޴����� �÷��� ��ư Ŭ���� ȣ��
        gameData = DataManager.Instance.data;
        newGame = true;
        if (selected > 0)
        {
            // ���� �ϱ�
            weaponType = selected;
        }
        else
        {
            // �̾� �ϱ�
            weaponType = gameData.weaponType;
            newGame = false;
        }
        PlayerPrefs.SetInt("weaponType", weaponType);
        DataManager.Instance.data.weaponType = weaponType;

        SceneManager.sceneLoaded += AfterLoading;

        if (!newGame) LoadingSceneController.LoadScene(gameData.sceneNumber, 3);
        else LoadingSceneController.LoadScene(1, 3);
    }
    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void ClearObjects()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i]);
        }
        objects.Clear();
    }
    public void AddToList(GameObject instance)
    {
        objects.Add(instance);
        DontDestroyOnLoad(instance);
    }
    private void AfterLoading(Scene scene, LoadSceneMode mode)
    {
        // �ε� �Ϸ��� ��ο� ȭ�鿡�� �׻� ȣ��Ǵ� �Լ�
        Debug.Log("���� �ε�Ǿ����ϴ�.");
        Debug.Log("�� �̸���" + scene.name + "�̸�");
        Debug.Log("�ε� Ÿ����" + loadingType + "�Դϴ�.");

        // ���� ������ �̵��� ��
        if (loadingType == 1)
        {
            // ���� ������ �÷��̾� �̵�
            TransportFinish();
        }
        // ���θ޴��� �̵��� ��
        else if (loadingType == 2)
        {
            StartCoroutine(ToMainMenu());
        }
        // ù��° �� �ε� ��
        else if (loadingType == 3)
        {
            OnFirstSceneLoaded();
        }
        else
        {
            // �ε� ���� �ε�Ǿ��� �� loadingType==0
            // Debug.Log("�ƹ��͵� ���մϴ�");
        }

        loadingType = 0;
    }
    private void OnFirstSceneLoaded()
    {
        var gameObject = Instantiate(players[weaponType - 1]);
        AddToList(gameObject);
        player = gameObject;
        player.name = players[weaponType - 1].name;
        DontDestroyOnLoad(player);

        if (!newGame)
        {
            // ���� �����Ͱ� �����ϴ� ��� ���� ��ġ��ǥ�� �÷��̾� ����.
            Vector3 pos = new Vector3();
            pos.x = gameData.position[0];
            pos.y = gameData.position[1];
            pos.z = gameData.position[2];
            player.transform.position = pos;
        }
        #region ī�޶� ��ġ �ʱ�ȭ
        tempPos = MainCamera.transform.position;
        tempPos.x = player.transform.position.x;
        tempPos.y = player.transform.position.y;

        MainCamera.transform.position = tempPos;

        difValue = MainCamera.transform.position - player.transform.position;
        difValue = new Vector3(Mathf.Abs(difValue.x), Mathf.Abs(difValue.y), 0f);
        #endregion

        #region �ʱ� ������Ʈ ��������
        gameObject = Instantiate(PlayerAfterImagePool);
        gameObject.name = PlayerAfterImagePool.name;
        AddToList(gameObject);

        gameObject = Instantiate(DamageTextPool);
        gameObject.name = DamageTextPool.name;
        AddToList(gameObject);

        gameObject = Instantiate(HitFxPool);
        gameObject.name = HitFxPool.name;
        AddToList(gameObject);

        gameObject = Instantiate(EnemyBulletPool);
        gameObject.name = EnemyBulletPool.name;
        AddToList(gameObject);

        if (weaponType == 1)
        {
            gameObject = Instantiate(SwordWindPool);
            gameObject.name = SwordWindPool.name;
            AddToList(gameObject);
        }
        else if (weaponType == 2)
        {
            gameObject = Instantiate(ArrowPool);
            gameObject.name = ArrowPool.name;
            AddToList(gameObject);

            gameObject = Instantiate(ArrowShowerPool);
            gameObject.name = ArrowShowerPool.name;
            AddToList(gameObject);
        }

        gameObject = Instantiate(ui_container);
        gameObject.name = ui_container.name;
        AddToList(gameObject);

        gameObject = Instantiate(EnemyPoolPrefab);
        gameObject.name = EnemyPoolPrefab.name;
        AddToList(gameObject);

        #endregion

        if (!newGame)
        {
            // �̾��ϱ��� ��� �α׸� ���� �����Ƽ ������ ����
            UI_Container.Instance.Data_Recovery();
        }

        isPlaying = true;
        Camera_Background_Container.SetActive(true);
        StartCoroutine(UI_Container.Instance.FadeInStart());
        StartCoroutine(UI_Container.Instance.StartSaving());

        DataManager.Instance.data.weaponType = PlayerPrefs.GetInt("weaponType");
        DataManager.Instance.SaveGameData();
    }
    public IEnumerator TransportFlow(Vector3 dest, bool loadScene)
    {
        destination = dest;
        StartCoroutine(UI_Container.Instance.FadeOutStart());
        // ���̵� �Ϸ�ñ��� ���
        yield return new WaitUntil(() => fadeState == "faded");
        isPlaying = false;
        if (loadScene)
        {
            // ���� �� �̵��� ���� �� ����
            EnemyPool.Instance.ClearRemainEnemies();
            LoadingSceneController.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, 1);
        }
        else
        {
            player.transform.position = destination;
            for (int i = 0; i < Camera_Background_Container.transform.childCount; i++)
            {
                var cam_background = Camera_Background_Container.transform.GetChild(i).gameObject;
                CameraBackgroundMover backgroundMover = cam_background.GetComponent<CameraBackgroundMover>();
                if (backgroundMover != null)
                {
                    backgroundMover.SettingOrigin();
                }
            }
            StartCoroutine(UI_Container.Instance.FadeInStart());
            isPlaying = true;
        }

        yield break;
    }
    private void TransportFinish()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length > 0)
        {
            int rand = Random.Range(0, spawnPoints.Length);
            player.transform.position = spawnPoints[rand].transform.position;
        }
        else player.transform.position = destination;

        for (int i = 0; i < Camera_Background_Container.transform.childCount; i++)
        {
            var cam_background = Camera_Background_Container.transform.GetChild(i).gameObject;
            CameraBackgroundMover backgroundMover = cam_background.GetComponent<CameraBackgroundMover>();
            if (backgroundMover != null)
            {
                backgroundMover.SettingOrigin();
            }
        }
        StartCoroutine(UI_Container.Instance.FadeInStart());
        isPlaying = true;
        //Debug.Log("�÷��̾� ���� ��");
    }
    public IEnumerator GiveUpFlow()
    {
        // ���̵� �Ϸ�ñ��� ���
        StartCoroutine(UI_Container.Instance.FadeOutStart());
        yield return new WaitUntil(() => fadeState == "faded");

        isPlaying = false;
        MainCamera.transform.position = new Vector3(0f, 0f, -10f);

        // ������ ���·� ǥ��
        DataManager.Instance.data.weaponType = -1;
        DataManager.Instance.SaveGameData();

        // ���� �� �̵��� ���� �� ����
        EnemyPool.Instance.ClearRemainEnemies();

        // �ε��� ȣ��
        LoadingSceneController.LoadScene(0, 2);
        yield break;
    }
    public IEnumerator QuitGameFlow()
    {
        // ���̵� �Ϸ�ñ��� ���
        StartCoroutine(UI_Container.Instance.FadeOutStart());
        yield return new WaitUntil(() => fadeState == "faded");

        isPlaying = false;
        MainCamera.transform.position = new Vector3(0f, 0f, -10f);

        // ���� �� �̵��� ���� �� ����
        EnemyPool.Instance.ClearRemainEnemies();

        // �ε��� ȣ��
        LoadingSceneController.LoadScene(0, 2);
        yield break;
    }
    private IEnumerator ToMainMenu()
    {
        // ȭ�� ���
        Camera_Background_Container.SetActive(false);
        // base UI ��Ȱ��ȭ
        UI_Container.Instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(UI_Container.Instance.FadeInStart());
        yield return new WaitUntil(() => fadeState == "clear");

        ClearObjects();
    }
    public void EnableSurpentScrew()
    {
        if (SerpentScrewPrefab != null)
        {
            GameObject screw = Instantiate(SerpentScrewPrefab);
            screw.name = SerpentScrewPrefab.name;
            screw.transform.SetParent(player.transform);
            screw.transform.position = player.GetComponent<BoxCollider2D>().bounds.center;
            screw.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}