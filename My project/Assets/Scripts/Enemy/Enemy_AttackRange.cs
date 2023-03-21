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
        string tag = collision.tag;
        if (tag == "Player")
        {
            //gameObject.GetComponentInParent<Enemy>().doAttack = true;
            //gameObject.GetComponentInParent<Enemy_Default>().playerInRange = true;
            enemy_Default.range[idx] = true;
            //gameObject.GetComponentInParent<Enemy_Default>().range[idx] = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            if (enemy_Default != null) enemy_Default.range[idx] = false;

            //gameObject.GetComponentInParent<Enemy_Default>().range[idx] = false;

            //gameObject.GetComponentInParent<Enemy_Default>().playerInRange = false;
            //gameObject.GetComponentInParent<Enemy>().canMove = true;
        }
    }
}