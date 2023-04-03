using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    [SerializeField] private EnemySummonData[] enemySummonDatas;

    [SerializeField] private GameObject[] enable_objective;

    private bool isEntered;
    private bool isStarted;

    private string sceneName;
    private int remainEnemies = 0;
    private void OnEnable()
    {
        isEntered = false;
        isStarted = false;
        sceneName = SceneManager.GetActiveScene().name;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isEntered)
        {
            isEntered = true;
            StartCoroutine(UI_Container.Instance.StartSaving());
            UI_Container.Instance.NoticeMainTextEnable("- " + sceneName + " -");

            DataManager.Instance.data.weaponType = PlayerPrefs.GetInt("weaponType");
            DataManager.Instance.SaveGameData();
            Invoke(nameof(EnemySpawn), 2f);
        }
    }
    private void EnemySpawn()
    {
        int enemy_type, enemy_gold;
        float enemy_maxHP;
        Transform enemy_trans;
        for (int i = 0; i < enemySummonDatas.Length; i++)
        {
            enemy_type = enemySummonDatas[i].enemy_type;
            enemy_gold = enemySummonDatas[i].drop_gold;
            enemy_maxHP = enemySummonDatas[i].maxHP;
            enemy_trans = enemySummonDatas[i].enemy_transform;

            var enemy = EnemyPool.Instance.GetFromPool(enemy_type);
            enemy.transform.position = enemy_trans.position;
            enemy.GetComponent<Enemy_Default>().EnemySetting(enemy_maxHP, enemy_gold);
            enemy.SetActive(true);
        }
        isStarted = true;

        if (enemySummonDatas.Length == 0)
        {
            UI_Container.Instance.NoticeSubTextEnable("");
            remainEnemies = enemySummonDatas.Length;
        }
        else
        {
            UI_Container.Instance.NoticeSubTextEnable("적을 처치하십시오\n(" + $"{enemySummonDatas.Length}/{enemySummonDatas.Length}" + ")");
            remainEnemies = enemySummonDatas.Length;
        }
    }
    private void Update()
    {
        if (isStarted)
        {
            if (remainEnemies != EnemyPool.Instance.GetEnemiesCount())
            {
                remainEnemies = EnemyPool.Instance.GetEnemiesCount();
                UI_Container.Instance.NoticeSubTextEnable("적을 처치하십시오\n(" + $"{remainEnemies}/{enemySummonDatas.Length}" + ")");
            }
            if (remainEnemies == 0)
            {
                for (int i = 0; i < enable_objective.Length; i++)
                {
                    enable_objective[i].SetActive(true);
                }
                // 스테이지 클리어 표시
                isStarted = false;
                if (enemySummonDatas.Length > 0) UI_Container.Instance.NoticeMainTextEnable("스테이지 클리어");
                UI_Container.Instance.NoticeSubTextEnable("");
            }
        }
    }
}
