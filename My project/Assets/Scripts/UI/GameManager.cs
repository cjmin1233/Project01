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
    public List<GameObject> objects;
    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject startCamera;
    [SerializeField] private GameObject PlayerAfterImagePool;
    [SerializeField] private GameObject DamageTextPool;
    [SerializeField] private GameObject HitFxPool;
    [SerializeField] private GameObject SwordWindPool;
    [SerializeField] private GameObject ArrowPool;
    [SerializeField] private GameObject ArrowShowerPool;
    [SerializeField] private GameObject ui_container;
    [SerializeField] private GameObject eventSystem;
    [HideInInspector] public bool newGame;
    int weaponType;
    Data gameData;
    private GameObject player;
    private GameObject boss;
    private GameObject MainCamera;
    private GameObject Camera_Background_Container;
    private Vector3 tempPos;
    [SerializeField] private float speed;
    [SerializeField] Vector3 difValue;

    public bool playerFollowing;
    public bool bossFollowing;
    public bool isPlaying;
    public int loadingType;

    // ���̵��� ȿ��, �ε�ȭ��
    public bool faded;
    private Vector3 destination;
    
    private void Awake()
    {
        #region ��ü ����ȭ
        var objs = FindObjectsOfType<GameManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion

        #region �ʱ� ����
        objects = new List<GameObject>();

        playerFollowing = true;
        bossFollowing = false;
        isPlaying = false;
        faded = false;
        loadingType = 0;

        var event_system = Instantiate(eventSystem);
        event_system.name = eventSystem.name;

        var cam_obj = Instantiate(startCamera);
        MainCamera = cam_obj;
        MainCamera.name = startCamera.name;
        Camera_Background_Container = MainCamera.transform.GetChild(0).gameObject;
        #endregion
    }
    private void FixedUpdate()
    {
        if (isPlaying)
        {
            float _size = MainCamera.GetComponent<Camera>().orthographicSize;
            if (bossFollowing)
            {
                MainCamera.GetComponent<Camera>().orthographicSize = _size > 3f ? _size - Time.deltaTime * 2f : 3f;
                if (boss == null) boss = GameObject.FindGameObjectWithTag("Boss");
                tempPos = Vector3.Lerp(MainCamera.transform.position, boss.transform.position + difValue, speed);
                MainCamera.transform.position = new Vector3(tempPos.x, tempPos.y, -10f);
            }
            else if (playerFollowing)
            {
                // �÷��̾� ���� ī�޶�
                MainCamera.GetComponent<Camera>().orthographicSize = _size < 5f ? _size + Time.deltaTime * 2f : 5f;
                tempPos = Vector3.Lerp(MainCamera.transform.position, player.transform.position + difValue, speed);
                MainCamera.transform.position = new Vector3(tempPos.x, tempPos.y, -10f);
            }
        }
    }
    public void PlayGame(int selected)
    {
        // ���� �޴����� �÷��� ��ư Ŭ���� ȣ��
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

        SceneManager.sceneLoaded += AfterLoading;
        //SceneManager.sceneLoaded += OnFirstSceneLoaded;

        //if(!newGame) SceneManager.LoadScene(gameData.sceneNumber);
        //else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        if(!newGame) LoadingSceneController.LoadScene(gameData.sceneNumber, 3);
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
        for(int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i]);
        }
        objects.Clear();
    }
    public void AddToList(GameObject instance)
    {
        objects.Add(instance);
    }
    private void AfterLoading(Scene scene, LoadSceneMode mode)
    {
        // �ε� �Ϸ��� ��ο� ȭ�鿡�� �׻� ȣ��Ǵ� �Լ�
        Debug.Log("���� �ε�Ǿ����ϴ�.");
        Debug.Log("�� �̸��� " + scene.name + "�̸�");
        Debug.Log("�ε� Ÿ���� " + loadingType + "�Դϴ�.");
        if (loadingType == 1)
        {
            // ���� ������ �÷��̾� �̵�
            TransportFinish();
        }
        else if (loadingType == 2)
        {
            // ���θ޴��� �̵�
            StartCoroutine(ToMainMenu());
        }
        else if (loadingType == 3)
        {
            // ù �� �ε� �� ����
            OnFirstSceneLoaded();
        }
        else Debug.Log("�ƹ��͵� ���մϴ�");

        loadingType = 0;
    }
    //Scene scene, LoadSceneMode mode
    private void OnFirstSceneLoaded()
    {
        var gameObject = Instantiate(players[weaponType - 1]);
        AddToList(gameObject);
        player = gameObject;
        player.name = players[weaponType - 1].name;

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
        #endregion

        if (!newGame)
        {
            // �̾��ϱ��� ��� �α׸� ���� �����Ƽ ������ ����
            UI_Container.Instance.Data_Recovery();
        }

        isPlaying = true;
        Camera_Background_Container.SetActive(true);
        StartCoroutine(UI_Container.Instance.FadeInStart());
    }
    public IEnumerator TransportFlow(Vector3 dest, bool loadScene)
    {
        destination = dest;
        faded = false;
        //StartCoroutine(UI_Container.Instance.FadeFlow());
        StartCoroutine(UI_Container.Instance.FadeOutStart());
        // ���̵� �Ϸ�ñ��� ���
        yield return new WaitUntil(() => faded);
        isPlaying = false;
        if (loadScene)
        {
            Debug.Log("�ε� ��ٸ�����..");
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

        for(int i = 0; i < Camera_Background_Container.transform.childCount; i++)
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
        Debug.Log("�÷��̾� ���� ��");
    }
    public IEnumerator GiveUpFlow()
    {
        // ���̵� �Ϸ�ñ��� ���
        faded = false;
        //StartCoroutine(UI_Container.Instance.FadeFlow());
        StartCoroutine(UI_Container.Instance.FadeOutStart());
        yield return new WaitUntil(() => faded);

        isPlaying = false;
        MainCamera.transform.position = new Vector3(0f, 0f, -10f);

        // ������ ���·� ǥ��
        DataManager.Instance.data.weaponType = -1;
        DataManager.Instance.SaveGameData();

        // �ε��� ȣ��
        LoadingSceneController.LoadScene(0, 2);
        yield break;
    }
    public IEnumerator QuitGameFlow()
    {
        // ���̵� �Ϸ�ñ��� ���
        faded = false;
        //StartCoroutine(UI_Container.Instance.FadeFlow());
        StartCoroutine(UI_Container.Instance.FadeOutStart());
        yield return new WaitUntil(() => faded);

        isPlaying = false;
        MainCamera.transform.position = new Vector3(0f, 0f, -10f);

        //������ ����
        DataManager.Instance.data.weaponType = PlayerPrefs.GetInt("weaponType");
        DataManager.Instance.data.sceneNumber = SceneManager.GetActiveScene().buildIndex;
        DataManager.Instance.SaveGameData();

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
        //UI_Container.Instance.fade_in_start = true;
        StartCoroutine(UI_Container.Instance.FadeInStart());
        yield return new WaitUntil(() => !faded);
        ClearObjects();
    }
}
