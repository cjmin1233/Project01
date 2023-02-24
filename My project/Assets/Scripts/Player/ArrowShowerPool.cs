using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShowerPool : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    //[HideInInspector] public Transform startpoint;

    public static ArrowShowerPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        //startpoint = transform;
        GrowPool();
    }
    private void GrowPool()
    {
        for (int i = 0; i < 5; i++)
        {
            var instanceToAdd = Instantiate(prefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            GrowPool();
        }
        var instance = availableObjects.Dequeue();
        //instance.SetActive(true);
        return instance;
    }

    /*public void Init_Startpoint(Transform transform)
    {
        startpoint.position = transform.position;
    }*/
}
