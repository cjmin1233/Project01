using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
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
            StartCoroutine(UI_Container.Instance.StartSaving());

            DataManager.Instance.data.weaponType = PlayerPrefs.GetInt("weaponType");
            DataManager.Instance.SaveGameData();
            Invoke(nameof(EnemySpawn), 2f);
        }
    }
    private void EnemySpawn()
    {
        for (int i = 0; i < enemies_type.Length; i++)
        {
            var enemy = EnemyPool.Instance.GetFromPool(enemies_type[i]);
            enemy.transform.position = enemies_trasform[i].position;
            enemy.SetActive(true);
        }
        isStarted = true;
        //GameManager.Instance.stageCleared = false;
        if (enemies_type.Length == 0) EnemyPool.Instance.remainEnemies = 0;
    }
    private void Update()
    {
        if (isStarted && EnemyPool.Instance.remainEnemies == 0)
        {
            for(int i = 0; i < enable_objective.Length; i++)
            {
                enable_objective[i].SetActive(true);
            }
            // �������� Ŭ���� ǥ��
            isStarted = false;
        }
    }
}
