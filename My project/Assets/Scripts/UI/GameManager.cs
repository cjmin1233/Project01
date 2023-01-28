using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Scene startScene;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //startScene = SceneManager.GetSceneByBuildIndex(0);
    }
    /*private void Update()
    {
        if (startScene.isLoaded) Debug.Log("start scene loaded");
    }*/
}
