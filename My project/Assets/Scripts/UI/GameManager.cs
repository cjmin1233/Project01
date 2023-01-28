using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<GameObject> objects;
    //Scene startScene;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //startScene = SceneManager.GetSceneByBuildIndex(0);
        objects = new List<GameObject>();
    }
    /*private void Update()
    {
        if (startScene.isLoaded) Debug.Log("start scene loaded");
    }*/
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
}
