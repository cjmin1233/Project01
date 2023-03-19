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
    [SerializeField] private GameObject EnemyBulletPool;
    [SerializeField] private GameObject SwordWindPool;
    [SerializeField] private GameObject ArrowPool;
    [SerializeField] private GameObject ArrowShowerPool;
    [SerializeField] private GameObject EnemyPool;
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
    //public bool stageCleared;

    // 페이드인 효과, 로딩화면
    public bool faded;
    private Vector3 destination;
    
    private void Awake()
    {
        #region 객체 직렬화
        var objs = FindObjectsOfType<GameManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion

        #region 초기 세팅
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
                // 플레이어 추적 카메라
                MainCamera.GetComponent<Camera>().orthographicSize = _size < 5f ? _size + Time.deltaTime * 2f : 5f;
                tempPos = Vector3.Lerp(MainCamera.transform.position, player.transform.position + difValue, speed);
                MainCamera.transform.position = new Vector3(tempPos.x, tempPos.y, -10f);
            }
        }
    }
    public void PlayGame(int selected)
    {
        // 메인 메뉴에서 플레이 버튼 클릭시 호출
        gameData = DataManager.Instance.data;
        newGame = true;
        if (selected > 0)
        {
            // 새로 하기
            weaponType = selected;
        }
        else
        {
            // 이어 하기
            weaponType = gameData.weaponType;
            newGame = false;
        }
        PlayerPrefs.SetInt("weaponType", weaponType);
        DataManager.Instance.data.weaponType = weaponType;

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
        // 로딩 완료후 어두운 화면에서 항상 호출되는 함수
        Debug.Log("씬이 로드되었습니다.");
        Debug.Log("씬 이름은 " + scene.name + "이며");
        Debug.Log("로드 타입은 " + loadingType + "입니다.");
        if (loadingType == 1)
        {
            // 다음 씬에서 플레이어 이동
            TransportFinish();
        }
        else if (loadingType == 2)
        {
            // 메인메뉴로 이동
            StartCoroutine(ToMainMenu());
        }
        else if (loadingType == 3)
        {
            // 첫 씬 로딩 후 세팅
            OnFirstSceneLoaded();
        }
        else Debug.Log("아무것도 안합니다");

        loadingType = 0;
    }
    private void OnFirstSceneLoaded()
    {
        var gameObject = Instantiate(players[weaponType - 1]);
        AddToList(gameObject);
        player = gameObject;
        player.name = players[weaponType - 1].name;

        if (!newGame)
        {
            // 게임 데이터가 존재하는 경우 이전 위치좌표에 플레이어 생성.
            Vector3 pos = new Vector3();
            pos.x = gameData.position[0];
            pos.y = gameData.position[1];
            pos.z = gameData.position[2];
            player.transform.position = pos;
        }
        #region 카메라 위치 초기화
        tempPos = MainCamera.transform.position;
        tempPos.x = player.transform.position.x;
        tempPos.y = player.transform.position.y;

        MainCamera.transform.position = tempPos;

        difValue = MainCamera.transform.position - player.transform.position;
        difValue = new Vector3(Mathf.Abs(difValue.x), Mathf.Abs(difValue.y), 0f);
        #endregion

        #region 초기 오브젝트 생성과정
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

        gameObject = Instantiate(EnemyPool);
        gameObject.name = EnemyPool.name;
        AddToList(gameObject);

        #endregion

        if (!newGame)
        {
            // 이어하기인 경우 로그를 통해 어빌리티 데이터 복구
            UI_Container.Instance.Data_Recovery();
        }

        isPlaying = true;
        Camera_Background_Container.SetActive(true);
        StartCoroutine(UI_Container.Instance.FadeInStart());
        //DataManager.Instance.data.weaponType = PlayerPrefs.GetInt("weaponType");
        DataManager.Instance.SaveGameData();
    }
    public IEnumerator TransportFlow(Vector3 dest, bool loadScene)
    {
        destination = dest;
        faded = false;
        StartCoroutine(UI_Container.Instance.FadeOutStart());
        // 페이드 완료시까지 대기
        yield return new WaitUntil(() => faded);
        isPlaying = false;
        if (loadScene)
        {
            Debug.Log("로딩 기다리는중..");
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
        Debug.Log("플레이어 전송 끝");
    }
    public IEnumerator GiveUpFlow()
    {
        // 페이드 완료시까지 대기
        faded = false;
        StartCoroutine(UI_Container.Instance.FadeOutStart());
        yield return new WaitUntil(() => faded);

        isPlaying = false;
        MainCamera.transform.position = new Vector3(0f, 0f, -10f);

        // 포기한 상태로 표시
        DataManager.Instance.data.weaponType = -1;
        DataManager.Instance.SaveGameData();

        // 로딩씬 호출
        LoadingSceneController.LoadScene(0, 2);
        yield break;
    }
    public IEnumerator QuitGameFlow()
    {
        // 페이드 완료시까지 대기
        faded = false;
        StartCoroutine(UI_Container.Instance.FadeOutStart());
        yield return new WaitUntil(() => faded);

        isPlaying = false;
        MainCamera.transform.position = new Vector3(0f, 0f, -10f);

        // 로딩씬 호출
        LoadingSceneController.LoadScene(0, 2);
        yield break;
    }
    private IEnumerator ToMainMenu()
    {
        // 화면 밝게
        Camera_Background_Container.SetActive(false);
        // base UI 비활성화
        UI_Container.Instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(UI_Container.Instance.FadeInStart());
        yield return new WaitUntil(() => !faded);
        ClearObjects();
    }
}
