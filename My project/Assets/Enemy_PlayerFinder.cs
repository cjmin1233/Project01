using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_PlayerFinder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            gameObject.GetComponentInParent<Enemy>().detectPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            gameObject.GetComponentInParent<Enemy>().detectPlayer = false;
        }
    }
}
