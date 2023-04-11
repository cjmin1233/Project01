using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
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
