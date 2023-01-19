using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackRange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            gameObject.GetComponentInParent<Enemy>().doAttack = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            gameObject.GetComponentInParent<Enemy>().doAttack = false;
            gameObject.GetComponentInParent<Enemy>().canMove = true;
        }
    }
}
