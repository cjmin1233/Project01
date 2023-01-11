using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Shower_Collider : MonoBehaviour
{
    [HideInInspector] public float damage;
    //[HideInInspector] public float anim_Speed = 1.0f;
    [HideInInspector] public Vector2 damageForce;
    //public AudioSource audioSource;
    //private Animator animator;
    //private List<string> hit_list;
    /*private void Awake()
    {
        //animator = GetComponent<Animator>();
        //hit_list = new List<string>();
    }*/
    /*private void OnEnable()
    {
        //animator.SetFloat("EnableSpeed", anim_Speed);
        //if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        //string name = collision.name;
        if (tag == "Enemy")
        {
            //Debug.Log("hit " + collision.name);
            collision.GetComponent<Enemy>().TakeDamage(damage, damageForce);
        }
        else if (tag == "Boss")
        {
            collision.GetComponent<Boss>().TakeDamage(damage);
        }
    }
    private void Disable_Sword_Collider()
    {
        ArrowShowerPool.Instance.AddToPool(gameObject);
    }
}
