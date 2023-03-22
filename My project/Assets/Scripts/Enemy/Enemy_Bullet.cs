using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    /*[HideInInspector] public float damage;
    [HideInInspector] public Vector2 bulletSpeed;
    [HideInInspector] public int type;
    private float speed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private CircleCollider2D circleCollider2D;
    //[SerializeField] private LayerMask playerLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        type = 0;
    }
    private void OnEnable()
    {
        animator.SetInteger("Type", type);

        circleCollider2D.enabled = false;//
        boxCollider2D.enabled = true;//
        rb.gravityScale = 0f;//
        if (type == 0)
        {
            boxCollider2D.isTrigger = true;//
            boxCollider2D.offset = Vector2.zero;//
            boxCollider2D.size = new Vector2(0.16f, 0.16f);//
            rb.velocity = new Vector2(transform.right.x * speed, 0f);//
        }
        else if (type == 1)
        {
            boxCollider2D.isTrigger = false;//
            boxCollider2D.offset = new Vector2(0.025f, -0.02f);//
            boxCollider2D.size = new Vector2(0.16f, 0.16f);//
            rb.gravityScale = 1f;//
            rb.velocity = new Vector2(transform.right.x * speed, 5f);//
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            collision.GetComponent<Player>().TakeDamage(damage);
        }
        animator.SetTrigger("Hit");
        rb.velocity = Vector2.zero;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetTrigger("Hit");
        rb.velocity = Vector2.zero;
    }
    private void Disable_Bullet()
    {
        EnemyBulletPool.Instance.AddToPool(gameObject);
    }
    private void Bomb_Explosion()
    {
        //boxCollider2D.isTrigger = true;
        //boxCollider2D.size = new Vector2(0.4f, 0.4f);
        boxCollider2D.enabled = false;
        circleCollider2D.enabled = true;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
    }*/
    [HideInInspector] public float damage;
    [HideInInspector] public Vector2 bulletSpeed;
    [HideInInspector] public int type;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        type = 0;
    }
    private void OnEnable()
    {
        animator.SetInteger("Type", type);
        rb.velocity = new Vector2(transform.right.x * bulletSpeed.x, transform.up.y * bulletSpeed.y);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            collision.GetComponent<Player>().TakeDamage(damage);
        }
        animator.SetTrigger("Hit");
        rb.velocity = Vector2.zero;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetTrigger("Hit");
        rb.velocity = Vector2.zero;
    }
    private void BulletStop()
    {
        rb.velocity = Vector2.zero;
    }
    private void Disable_Bullet()
    {
        EnemyBulletPool.Instance.AddToPool(gameObject);
    }
}
