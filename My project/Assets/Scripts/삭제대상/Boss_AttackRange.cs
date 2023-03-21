using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_AttackRange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            gameObject.GetComponentInParent<Boss>().playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            gameObject.GetComponentInParent<Boss>().playerInRange = false;
        }
    }

}
