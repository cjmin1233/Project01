using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Combo_Collider : MonoBehaviour
{
    public float damage;
    public float anim_Speed = 1.0f;
    public AudioSource audioSource;

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {        
        animator.SetFloat("EnableSpeed", anim_Speed);
        if (audioSource!=null) audioSource.PlayOneShot(audioSource.clip);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Enemy")
        {
            Debug.Log("hit " + collision.name);
            collision.GetComponent<Enemy>().TakeDamage(damage);
        }
        else if (tag == "Boss")
        {
            collision.GetComponent<Boss>().TakeDamage(damage);
        }
        else Debug.Log("We hit " + collision.name);
    }
    private void Disable_Sword_Collider()
    {
        gameObject.SetActive(false);
    }
}
