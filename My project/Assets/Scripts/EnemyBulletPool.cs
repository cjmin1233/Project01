using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletPool : MonoBehaviour
{
    [SerializeField] private GameObject Prefab;
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static EnemyBulletPool Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
        GrowPool();
    }
    private void GrowPool()
    {
        for (int i = 0; i < 5; i++)
        {
            var instanceToAdd = Instantiate(Prefab);
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

}
