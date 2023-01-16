using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill : MonoBehaviour
{
    public GameObject target;
    GameObject player;
    public int damage;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            //player.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
    }
    private void Fin_Ani()
    {
        //GetComponentInParent<GameObject>().SetActive(false);
        target.gameObject.SetActive(false);
    }

}
