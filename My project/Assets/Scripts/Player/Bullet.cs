using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float anim_Speed;
    private float speed = 25f;
    //public GameObject ImpactEffect;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim_Speed = 1.0f;
    }
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //transform.position = player.position;
        transform.rotation = player.rotation;
        //
        animator.SetFloat("EnableSpeed", anim_Speed);
        rb.velocity = transform.right * speed;
    }
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        string tag = hitInfo.tag;
        if (tag == "Enemy")
        {
            hitInfo.GetComponent<Enemy>().TakeDamage(damage, Vector2.zero);
        }
        else if (tag == "Boss")
        {
            Debug.Log("We hit the " + hitInfo.name);
            hitInfo.GetComponent<Boss>().TakeDamage(damage);
        }
        /*else
        {
            Debug.Log("We hit nothing");
        }*/
        //destroyBullet();
        Disable_Arrow();
    }
    private void Disable_Arrow()
    {
        ArrowPool.Instance.AddToPool(gameObject);
    }
}
