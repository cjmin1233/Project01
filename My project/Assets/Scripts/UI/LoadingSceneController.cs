using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingSceneController : MonoBehaviour
{
    public static LoadingSceneController instance;
    static int nextScene;
    static int loadingType; // 로딩 완료 후 할일
    [SerializeField] Image progressBar;

    public static void LoadScene(int sceneIndex, int loading_type)
    {
        nextScene = sceneIndex;
        loadingType = loading_type;
        Debug.Log("로딩씬 로드");
        SceneManager.LoadScene("LoadingScene"); // Start() 에서 코루틴 LoadSceneProcess 시작
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress) timer = 0f;
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progressBar.fillAmount >= 1f && !op.allowSceneActivation)
                {
                    op.allowSceneActivation = true;
                    // 로딩 완료 후 행동 지정
                    GameManager.Instance.loadingType = loadingType;
                    //Debug.Log("로딩 완료?" + timer);
                }
            }
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
