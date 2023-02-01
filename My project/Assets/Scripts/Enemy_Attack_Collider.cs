using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_Collider : MonoBehaviour
{
    public float damage;
    public AudioSource audioSource;
    //public float anim_Speed = 1.0f;

    //private Animator animator;
    /*private void Awake()
    {
        animator = GetComponent<Animator>();
    }*/
    private void OnEnable()
    {
        //animator.SetFloat("EnableSpeed", anim_Speed);
        if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            //Debug.Log("hit " + collision.name);
            collision.GetComponent<Player>().TakeDamage(damage);
        }
        //else Debug.Log("We hit " + collision.name);
    }
}
