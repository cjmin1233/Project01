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
    int weaponType;
    [HideInInspector] public bool newGame;
    Data gameData;

    // ���̵��� ȿ��, �ε�ȭ��
    public bool faded;
    public bool loadingFinished;
    
    private void Awake()
    {
        /*if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Debug.Log("�ߺ��� gamemanager �߰�");
        }*/
        var objs = FindObjectsOfType<GameManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        objects = new List<GameObject>();

        faded = false;
        loadingFinished = false;
    }
    public void PlayGame(int selected)
    {
        gameData = DataManager.Instance.data;
        newGame = true;
        if (selected > 0)
        {
            // ���� �ϱ�
            PlayerPrefs.SetInt("weaponType", selected);
            weaponType = selected;
        }
        else
        {
            // �̾� �ϱ�
            weaponType = gameData.weaponType;
            PlayerPrefs.SetInt("weaponType", weaponType);
            newGame = false;
        }

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
        //Debug.Log(instance.name);
        objects.Add(instance);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OnScenceLoaded : " + scene.name);
        //Debug.Log(mode);
        var gameObject = Instantiate(players[weaponType - 1]);
        AddToList(gameObject);
        if (!newGame)
        {
            // ���� �����Ͱ� �����ϴ� ��� ���� ��ġ��ǥ�� �÷��̾� ����.
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
    public IEnumerator TransportFlow(Vector3 destination, bool loadScene)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        faded = false;
        /*IEnumerator fadeflow = UI_Container.Instance.FadeFlow();
        StartCoroutine(fadeflow);*/
        StartCoroutine(UI_Container.Instance.FadeFlow());

        yield return new WaitUntil(() => faded);
        Debug.Log("���̵� �Ϸ�");
        //if (loadScene) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if (loadScene)
        {
            loadingFinished = false;
            //SceneManager.LoadScene("LoadingScene");
            //
            LoadingSceneController.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("�ε� ��ٸ�����..");
            //yield return StartCoroutine(UI_Container.Instance.LoadSceneProcess(SceneManager.GetActiveScene().buildIndex + 1));
            //yield return new WaitUntil(() => loadingFinished == true);
            while (!loadingFinished)
            {
                yield return null;
            }
            //yield return null;
            Debug.Log("�÷��̾� ����");
            player.transform.position = destination;
        }
        else player.transform.position = destination;
        //GameObject.FindGameObjectWithTag("Player").transform.position = destination;
        UI_Container.Instance.fade_in_start = true;
        Debug.Log("�÷��̾� ���� ��");
        yield break;
    }
    public IEnumerator GiveUpFlow()
    {
        ClearObjects();
        // ������ ���·� ǥ��
        DataManager.Instance.data.weaponType = -1;
        DataManager.Instance.SaveGameData();

        //SceneManager.LoadScene(0);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(0);
        yield break;
    }
    public IEnumerator QuitGameFlow()
    {
        ClearObjects();

        DataManager.Instance.data.weaponType = PlayerPrefs.GetInt("weaponType");
        DataManager.Instance.data.sceneNumber = SceneManager.GetActiveScene().buildIndex;
        DataManager.Instance.SaveGameData();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(0);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            Debug.Log("���൵ : " + progress * 100f);
            if (progress >= 0.9f)
            {
                Debug.Log("hey");
                asyncOperation.allowSceneActivation = true;
            }
            else Debug.Log("isDone? : " + asyncOperation.isDone);
            yield return null;
        }
        //SceneManager.LoadScene(0);
        Debug.Log("�����ߴ� �Ϸ�");
        yield break;
    }
}
