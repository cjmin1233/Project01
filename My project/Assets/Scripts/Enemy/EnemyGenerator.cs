using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public int[] enemies_type;
    public Transform[] enemies_trasform;
    public GameObject[] enable_objective;

    //public GameObject[] enemies;
    private bool isEntered;
    private bool isStarted;
    private void OnEnable()
    {
        isEntered = false;
        isStarted = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isEntered)
        {
            isEntered = true;
            for(int i = 0; i < enemies_type.Length; i++)
            {
                var enemy = EnemyPool.Instance.GetFromPool(enemies_type[i]);
                enemy.transform.position = enemies_trasform[i].position;
                enemy.SetActive(true);
            }
            isStarted = true;
        }
    }
    private void Update()
    {
        if (isStarted && EnemyPool.Instance.remainEnemies == 0)
        {
            for(int i = 0; i < enable_objective.Length; i++)
            {
                enable_objective[i].SetActive(true);
            }
            // 스테이지 클리어 표시
        }
    }
}
