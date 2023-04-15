using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackRange : MonoBehaviour
{
    private Enemy_Default enemy_Default;
    [SerializeField] private int idx;
    private void OnEnable()
    {
        enemy_Default = GetComponentInParent<Enemy_Default>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy_Default.range[idx] = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (enemy_Default != null) enemy_Default.range[idx] = false;
        }
    }
}