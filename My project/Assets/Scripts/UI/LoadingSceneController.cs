using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingSceneController : MonoBehaviour
{
    static int nextScene;
    [SerializeField] Image progressBar;

    public static void LoadScene(int sceneIndex)
    {
        nextScene = sceneIndex;
        Debug.Log("�ε��� �ε�");
        SceneManager.LoadScene("LoadingScene");
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
            /*yield return null;

            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progressBar.fillAmount >= 1f)
                {
                    GameManager.Instance.loadingFinished = true;
                    Debug.Log(SceneManager.GetActiveScene().name);
                    Debug.Log("�ε� �Ϸ�");
                    op.allowSceneActivation = true;
                    yield break;
                }
            }*/
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            if (progress < 0.9f) progressBar.fillAmount = progress;
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    Debug.Log("here");
                    break;
                }
            }
            //Debug.Log("�ε��� ������Ʈ : " + progress * 100f + "%");
            //progressBar.fillAmount = progress;
            //if (op.progress >= 0.9f) op.allowSceneActivation = true;
            yield return null;
        }

        //yield return new WaitForSeconds(0.5f);
        Debug.Log("�ε� �Ϸ� : " + op.isDone);
        progressBar.fillAmount = 1f;
        GameManager.Instance.loadingFinished = true;
        yield break;
    }
}
