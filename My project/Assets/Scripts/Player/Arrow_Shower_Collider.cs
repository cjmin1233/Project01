using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Shower_Collider : MonoBehaviour
{
    [HideInInspector] public float damage;
    //[HideInInspector] public float anim_Speed = 1.0f;
    [HideInInspector] public Vector2 damageForce;
    [HideInInspector] public bool rain_enable = false;
    [HideInInspector] public bool slow_enable = false;
    private float damage_multiplier = 1f;
    //public AudioSource audioSource;
    //private Animator animator;
    //private List<string> hit_list;
    /*private void Awake()
    {
        //animator = GetComponent<Animator>();
        //hit_list = new List<string>();
    }*/
    private void OnEnable()
    {
        damage_multiplier = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        float currDamage = Mathf.Round(damage * damage_multiplier);
        if (tag == "Enemy")
        {
            collision.GetComponent<Enemy>().TakeDamage(currDamage, damageForce);
        }
        else if (tag == "Boss")
        {
            collision.GetComponent<Boss>().TakeDamage(currDamage);
        }
        if(tag=="Enemy" || tag == "Boss")
        {
            if (damage_multiplier < 1.6f && rain_enable) damage_multiplier += 0.1f;
        }
    }
    private void Disable_Sword_Collider()
    {
        ArrowShowerPool.Instance.AddToPool(gameObject);
    }
}
