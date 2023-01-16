using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo_Collider : MonoBehaviour
{
    [HideInInspector] public float damage;
    //[HideInInspector] public float anim_Speed = 1.0f;
    [HideInInspector] public Vector2 damageForce;
    //public AudioSource audioSource;
    //private Animator animator;
    private List<string> hit_list;
    private void Awake()
    {
        //animator = GetComponent<Animator>();
        hit_list = new List<string>();
    }
    /*private void OnEnable()
    {
        //animator.SetFloat("EnableSpeed", anim_Speed);
        //if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;
        if (!hit_list.Contains(name))
        {
            hit_list.Add(name);
            if (tag == "Enemy")
            {
                //Debug.Log("hit " + collision.name);
                collision.GetComponent<Enemy>().TakeDamage(damage, damageForce);
                //Debug.Log("x,y collider offset: " + collision.offset.x + " " + collision.offset.y);
            }
            else if (tag == "Boss")
            {
                collision.GetComponent<Boss>().TakeDamage(damage);
            }
            //else Debug.Log("We hit " + collision.name);
        }
    }
    private void OnDisable()
    {
        hit_list.Clear();
    }
    /*private void Disable_Sword_Collider()
    {
        gameObject.SetActive(false);
        hit_list.Clear();
    }*/
}
