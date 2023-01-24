using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float anim_Speed;
    [HideInInspector] public bool isPoisoned;
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
        isPoisoned = false;
    }
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //transform.position = player.position;
        transform.rotation = player.rotation;
        animator.SetBool("IsPoisoned", isPoisoned);
        //animator.SetFloat("EnableSpeed", anim_Speed);
        rb.velocity = transform.right * speed;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Enemy")
        {
            collision.GetComponent<Enemy>().TakeDamage(damage, Vector2.zero);
            if (isPoisoned) collision.GetComponent<Enemy>().TakeDamage(1f, Vector2.zero);

        }
        else if (tag == "Boss")
        {
            collision.GetComponent<Boss>().TakeDamage(damage);
            if (isPoisoned) collision.GetComponent<Boss>().TakeDamage(1f);
        }
        animator.SetTrigger("Hit");
        rb.velocity = Vector2.zero;
        //transform.SetParent(collision.gameObject.transform);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Enemy" || tag == "Boss")
        {
            // 적 충돌시 박힌채 이동
            rb.velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
        }
    }
    private void Disable_Arrow()
    {
        ArrowPool.Instance.AddToPool(gameObject);
    }
}
