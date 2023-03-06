using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFxPool : MonoBehaviour
{
    [SerializeField] private GameObject FxPrefab;
    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    public static HitFxPool Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
        
    }
    private void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(FxPrefab);
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
        return instance;
    }

}
