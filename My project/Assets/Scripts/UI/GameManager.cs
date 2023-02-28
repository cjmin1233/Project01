using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //static GameObject container;
    public static GameManager Instance;
    /*public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                container = new GameObject();
                container.name = "GameManager";
                instance = container.AddComponent(typeof(GameManager)) as GameManager;
                DontDestroyOnLoad(container);
            }
            return instance;
        }
    }*/
    public List<GameObject> objects;
    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject startCamera;
    [SerializeField] private GameObject PlayerAfterImagePool;
    [SerializeField] private GameObject DamageTextPool;
    [SerializeField] private GameObject SwordWindPool;
    [SerializeField] private GameObject ArrowPool;
    [SerializeField] private GameObject ArrowShowerPool;
    [SerializeField] private GameObject ui_container;
    int weaponType;
    [HideInInspector] public bool newGame;
    Data gameData;

    // 페이드인 효과
    [HideInInspector] public bool faded = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        objects = new List<GameObject>();
    }
    public void PlayGame(int selected)
    {
        gameData = DataManager.Instance.data;
        newGame = true;
        if (selected > 0)
        {
            // 새로 하기
            PlayerPrefs.SetInt("weaponType", selected);
            weaponType = selected;
        }
        else
        {
            // 이어 하기
            weaponType = gameData.weaponType;
            PlayerPrefs.SetInt("weaponType", weaponType);
            newGame = false;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if(!newGame) SceneManager.LoadScene(gameData.sceneNumber);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
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
        Debug.Log(instance.name);
        objects.Add(instance);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnScenceLoaded : " + scene.name);
        Debug.Log(mode);
        var gameObject = Instantiate(players[weaponType - 1]);
        AddToList(gameObject);
        if (!newGame)
        {
            // 게임 데이터가 존재하는 경우 이전 위치좌표에 플레이어 생성.
            Vector3 pos = new Vector3();
            pos.x = gameData.position[0];
            pos.y = gameData.position[1];
            pos.z = gameData.position[2];
            gameObject.transform.position = pos;
        }
        //DataManager.Instance.SaveGameData();

        gameObject = Instantiate(startCamera);
        AddToList(gameObject);
        gameObject = Instantiate(PlayerAfterImagePool);
        AddToList(gameObject);
        gameObject = Instantiate(DamageTextPool);
        AddToList(gameObject);

        if (weaponType == 1)
        {
            gameObject = Instantiate(SwordWindPool);
            AddToList(gameObject);
        }
        else if (weaponType == 2)
        {
            gameObject = Instantiate(ArrowPool);
            AddToList(gameObject);
            gameObject = Instantiate(ArrowShowerPool);
            AddToList(gameObject);
        }

        gameObject = Instantiate(ui_container);
        AddToList(gameObject);

        if (!newGame)
        {
            UI_Container.Instance.Data_Recovery();
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    /*public void PlayerTransport(Vector3 dest)
    {
        UI_Container.Instance.StartFadeFlow();
    }*/
    public IEnumerator TransportFlow(Vector3 destination, bool loadScene)
    {
        StartCoroutine(UI_Container.Instance.FadeFlow());
        yield return new WaitUntil(() => faded);

        GameObject.FindGameObjectWithTag("Player").transform.position = destination;
        //if (loadScene) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if (loadScene)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

            yield return new WaitUntil(() => asyncLoad.isDone);
        }

        UI_Container.Instance.fade_in_start = true;
    }
}
