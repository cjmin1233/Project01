using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingSceneController : MonoBehaviour
{
    static int nextScene;
    public static LoadingSceneController instance;
    [SerializeField] Image progressBar;

    public static void LoadScene(int sceneIndex)
    {
        nextScene = sceneIndex;
        Debug.Log("로딩씬 로드");
        SceneManager.LoadScene("LoadingScene");
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

            /*if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    GameManager.Instance.loadingFinished = true;
                    yield break;
                }
            }*/
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress) timer = 0f;
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progressBar.fillAmount >= 1f)
                {
                    //GameManager.Instance.loadingFinished = true;
                    op.allowSceneActivation = true;
                    Debug.Log("로딩 완료?" + timer);
                    Debug.Log("is Done ? : " + op.isDone);
                    GameManager.Instance.TransportFinish();
                    gameObject.SetActive(false);
                    yield break;
                }
            }
        }
        /*Debug.Log("로딩 완료 : " + op.isDone);
        progressBar.fillAmount = 1f;
        yield break;*/
        //Destroy(gameObject);
    }
}
