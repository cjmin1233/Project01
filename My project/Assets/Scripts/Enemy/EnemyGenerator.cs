using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    private bool isEntered;
    private void OnEnable()
    {
        isEntered = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isEntered)
        {
            isEntered = true;
            for(int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(true);
            }
        }
    }
}
