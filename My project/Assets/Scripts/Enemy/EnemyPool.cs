using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private class MultiQueue<GameObject>
    {
        private Queue<GameObject>[] queues;

        public MultiQueue(int count)
        {
            queues = new Queue<GameObject>[count];
            for(int i = 0; i < count; i++) { queues[i] = new Queue<GameObject>(); }
        }
        public void Enqueue(int index, GameObject item)
        {
            queues[index].Enqueue(item);
        }
        public GameObject Dequeue(int index)
        {
            return queues[index].Dequeue();
        }
        public int Count(int index)
        {
            return queues[index].Count;
        }
        public GameObject Peek(int index)
        {
            return queues[index].Peek();
        }
    }
    [SerializeField] private GameObject[] enemyPrefab;
    private MultiQueue<GameObject> enemyQueue;
    public int remainEnemies;
    public static EnemyPool Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //
        enemyQueue = new MultiQueue<GameObject>(enemyPrefab.Length);
        for(int i = 0; i < enemyPrefab.Length; i++)
        {
            GrowPool(i);
        }
        remainEnemies = -1;
    }
    private void GrowPool(int index)
    {
        for (int i = 0; i < 3; i++)
        {
            var instanceToAdd = Instantiate(enemyPrefab[index]);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(index, instanceToAdd);
        }
        /*for (int i = 0; i < 3; i++)
        {
            var instanceToAdd = Instantiate(mushroomPrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }*/
    }

    public void AddToPool(int index, GameObject instance)
    {
        instance.GetComponent<Enemy_Default>().enemyType = index;
        instance.SetActive(false);
        enemyQueue.Enqueue(index, instance);
        remainEnemies--;
        //mushroomQueue.Enqueue(instance);
    }

    public GameObject GetFromPool(int index)
    {
        if (enemyQueue.Count(index) == 0) GrowPool(index);
        var instance = enemyQueue.Dequeue(index);
        if (remainEnemies < 0) remainEnemies = 0;
        remainEnemies++;
        return instance;
    }
}
