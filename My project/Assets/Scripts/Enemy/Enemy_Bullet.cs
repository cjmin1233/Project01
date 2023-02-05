using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    private float speed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        rb.velocity = new Vector2(transform.right.x * speed, 0f);
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
    private void Disable_Bullet()
    {
        EnemyBulletPool.Instance.AddToPool(gameObject);
    }
}
