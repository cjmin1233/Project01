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
    [SerializeField] private GameObject SwordWindPool;
    [SerializeField] private GameObject ArrowPool;
    [SerializeField] private GameObject ArrowShowerPool;
    [SerializeField] private GameObject ui_container;
    [HideInInspector] public bool newGame;
    int weaponType;
    Data gameData;
    private GameObject player;
    private GameObject MainCamera;
    private GameObject Camera_Background_Container;
    private Vector3 tempPos;
    [SerializeField] private float speed;
    [SerializeField] Vector3 difValue;

    public bool playerFollowing;
    public bool isPlaying;
    public int loadingType;

    // 페이드인 효과, 로딩화면
    public bool faded;
    public Vector3 destination;
    
    private void Awake()
    {
        var objs = FindObjectsOfType<GameManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        objects = new List<GameObject>();

        playerFollowing = true;
        isPlaying = false;
        faded = false;
        loadingType = 0;

        var cam_obj = Instantiate(startCamera);
        MainCamera = cam_obj;
        MainCamera.name = startCamera.name;
        Camera_Background_Container = MainCamera.transform.GetChild(0).gameObject;
    }
    private void FixedUpdate()
    {
        if (isPlaying)
        {
            if (playerFollowing)
            {
                tempPos = Vector3.Lerp(MainCamera.transform.position, player.transform.position + difValue, speed);
                MainCamera.transform.position = new Vector3(tempPos.x, tempPos.y, -10f);
            }
        }
    }
    public void PlayGame(int selected)
    {
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

        SceneManager.sceneLoaded += TestSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(!newGame) SceneManager.LoadScene(gameData.sceneNumber);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
    private void TestSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("씬이 로드되었습니다.");
        Debug.Log("씬 이름은 " + scene.name + "이며");
        Debug.Log("로드 타입은 " + loadingType + "입니다.");
        if (loadingType == 1)
        {
            // 다음 씬으로 이동
            TransportFinish();
        }
        else if (loadingType == 2)
        {
            // 메인메뉴로 이동
            StartCoroutine(ToMainMenu());
        }
        else Debug.Log("아무것도 안합니다");
        loadingType = 0;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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
        //DataManager.Instance.SaveGameData();

        //gameObject = Instantiate(startCamera);
        //AddToList(gameObject);
        //MainCamera = gameObject;    // 카메라 오브젝트 할당

        #region 카메라 위치 초기화
        tempPos = MainCamera.transform.position;
        tempPos.x = player.transform.position.x;
        tempPos.y = player.transform.position.y;

        MainCamera.transform.position = tempPos;

        difValue = MainCamera.transform.position - player.transform.position;
        difValue = new Vector3(Mathf.Abs(difValue.x), Mathf.Abs(difValue.y), 0f);
        #endregion
        

        gameObject = Instantiate(PlayerAfterImagePool);
        gameObject.name = PlayerAfterImagePool.name;
        AddToList(gameObject);
        gameObject = Instantiate(DamageTextPool);
        gameObject.name = DamageTextPool.name;
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

        if (!newGame)
        {
            // 이어하기인 경우 로그를 통해 어빌리티 데이터 복구
            UI_Container.Instance.Data_Recovery();
        }

        isPlaying = true;
        Camera_Background_Container.SetActive(true);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public IEnumerator TransportFlow(Vector3 dest, bool loadScene)
    {
        destination = dest;
        faded = false;
        StartCoroutine(UI_Container.Instance.FadeFlow());
        // 페이드 완료시까지 대기
        yield return new WaitUntil(() => faded);
        isPlaying = false;
        if (loadScene)
        {
            //loadingType = 1;
            Debug.Log("로딩 기다리는중..");
            LoadingSceneController.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, 1);
        }
        else
        {
            player.transform.position = destination;
            UI_Container.Instance.fade_in_start = true;
            isPlaying = true;
        }

        yield break;
    }
    private void TransportFinish()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        player.transform.position = destination;
        if (spawnPoint != null) player.transform.position = spawnPoint.transform.position;
        
        // 화면 밝게
        UI_Container.Instance.fade_in_start = true;
        isPlaying = true;
        Debug.Log("플레이어 전송 끝");
    }
    public IEnumerator GiveUpFlow()
    {
        faded = false;
        StartCoroutine(UI_Container.Instance.FadeFlow());
        // 페이드 완료시까지 대기
        yield return new WaitUntil(() => faded);
        isPlaying = false;
        Vector3 Origin = Vector3.zero;
        Origin.z = -10f;
        MainCamera.transform.position = Origin;
        // 포기한 상태로 표시
        DataManager.Instance.data.weaponType = -1;
        DataManager.Instance.SaveGameData();

        LoadingSceneController.LoadScene(0, 2);
        //ClearObjects();
        yield break;
    }
    public IEnumerator QuitGameFlow()
    {
        faded = false;
        StartCoroutine(UI_Container.Instance.FadeFlow());
        // 페이드 완료시까지 대기
        yield return new WaitUntil(() => faded);
        isPlaying = false;
        Vector3 Origin = Vector3.zero;
        Origin.z = -10f;
        MainCamera.transform.position = Origin;
        //데이터 저장
        DataManager.Instance.data.weaponType = PlayerPrefs.GetInt("weaponType");
        DataManager.Instance.data.sceneNumber = SceneManager.GetActiveScene().buildIndex;
        DataManager.Instance.SaveGameData();

        LoadingSceneController.LoadScene(0, 2);
        yield break;
    }
    private IEnumerator ToMainMenu()
    {
        // 화면 밝게
        Camera_Background_Container.SetActive(false);
        UI_Container.Instance.fade_in_start = true;
        UI_Container.Instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitUntil(() => !faded);
        ClearObjects();
        Debug.Log("플레이어 전송 끝");
    }
}
