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
    private int[] enemy_id_counter;
    private MultiQueue<GameObject> enemyQueue;
    //public int remainEnemies;

    // 남은 적 관리
    public List<GameObject> remainEnemiesList;
    //public Queue<GameObject> ramainEnemiesQueue;
    public static EnemyPool Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
        //
        enemyQueue = new MultiQueue<GameObject>(enemyPrefab.Length);
        remainEnemiesList = new List<GameObject>();
        //ramainEnemiesQueue = new Queue<GameObject>();

        enemy_id_counter = new int[enemyPrefab.Length];

        for(int i = 0; i < enemyPrefab.Length; i++)
        {
            enemy_id_counter[i] = 0;
            GrowPool(i);
        }
        //remainEnemies = -1;
    }
    private void GrowPool(int index)
    {
        for (int i = 0; i < 3; i++)
        {
            var instanceToAdd = Instantiate(enemyPrefab[index]);
            instanceToAdd.name = enemyPrefab[index].name + "_" + (enemy_id_counter[index]++).ToString();
            instanceToAdd.transform.SetParent(transform);
            AddToPool(index, instanceToAdd);
        }
    }

    public void AddToPool(int index, GameObject instance)
    {
        instance.GetComponent<Enemy_Default>().enemyType = index;
        instance.SetActive(false);
        enemyQueue.Enqueue(index, instance);
        if (remainEnemiesList.Contains(instance)) remainEnemiesList.Remove(instance);
    }

    public GameObject GetFromPool(int index)
    {
        if (enemyQueue.Count(index) == 0) GrowPool(index);
        var instance = enemyQueue.Dequeue(index);
        //if (remainEnemies < 0) remainEnemies = 0;
        //remainEnemies++;
        //ramainEnemiesQueue.Enqueue(instance);
        remainEnemiesList.Add(instance);
        return instance;
    }
    public int GetEnemiesCount()
    {
        return remainEnemiesList.Count;
    }
    public void ClearRemainEnemies()
    {
        for (int i = remainEnemiesList.Count - 1; i >= 0; i--)
        {
            remainEnemiesList[i].GetComponent<Enemy_Default>().Eliminate();
        }
    }
}
