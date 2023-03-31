using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public int[] enemies_type;
    public Transform[] enemies_trasform;
    public GameObject[] enable_objective;

    //public GameObject[] enemies;
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
        for (int i = 0; i < enemies_type.Length; i++)
        {
            var enemy = EnemyPool.Instance.GetFromPool(enemies_type[i]);
            enemy.transform.position = enemies_trasform[i].position;
            enemy.SetActive(true);
        }
        isStarted = true;

        if (enemies_type.Length == 0)
        {
            //
        }
        else
        {
            UI_Container.Instance.NoticeSubTextEnable("���� óġ�Ͻʽÿ�\n(" + $"{enemies_type.Length}/{enemies_type.Length}" + ")");
            remainEnemies = enemies_type.Length;
        }
    }
    private void Update()
    {
        if (isStarted)
        {
            if (remainEnemies != EnemyPool.Instance.GetEnemiesCount())
            {
                remainEnemies = EnemyPool.Instance.GetEnemiesCount();
                UI_Container.Instance.NoticeSubTextEnable("���� óġ�Ͻʽÿ�\n(" + $"{remainEnemies}/{enemies_type.Length}" + ")");
            }
            if (remainEnemies == 0)
            {
                for (int i = 0; i < enable_objective.Length; i++)
                {
                    enable_objective[i].SetActive(true);
                }
                // �������� Ŭ���� ǥ��
                isStarted = false;
                if (enemies_type.Length > 0) UI_Container.Instance.NoticeMainTextEnable("�������� Ŭ����");
                UI_Container.Instance.NoticeSubTextEnable("");
            }
        }
    }
}
