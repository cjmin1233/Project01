using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_PlayerFinder : MonoBehaviour
{
    private Enemy_Default enemy_Default;
    private void OnEnable()
    {
        enemy_Default = GetComponentInParent<Enemy_Default>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            enemy_Default.detectPlayer = true;
            //gameObject.GetComponentInParent<Enemy_Default>().detectPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            if (enemy_Default != null) enemy_Default.detectPlayer = false;
            //gameObject.GetComponentInParent<Enemy_Default>().detectPlayer = false;
        }
    }
}
