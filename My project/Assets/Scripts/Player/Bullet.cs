using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int fxType;
    [HideInInspector] public float damage;
    [HideInInspector] public Vector2 damageForce;
    [HideInInspector] public bool isPoisoned;
    [HideInInspector] public bool isDiagonal;
    private float speed = 25f;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private List<string> hit_list;
    private Vector3 offset;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isPoisoned = false;
        isDiagonal = false;
        hit_list = new List<string>();
    }
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.rotation = player.rotation;
        animator.SetBool("IsPoisoned", isPoisoned);
        animator.SetBool("IsDiagonal", isDiagonal);
        if (isDiagonal) rb.velocity = new Vector2(transform.right.x * speed, transform.up.y * (-speed));
        else rb.velocity = transform.right * speed;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;
        if (!hit_list.Contains(name))
        {
            hit_list.Add(name);
            if (tag == "Enemy" || tag == "Boss")
            {
                if (isPoisoned)
                {
                    collision.GetComponent<Enemy_Default>().TakeDamage(damage, damageForce, false, Color.magenta, fxType + 1);
                    collision.GetComponent<Enemy_Default>().Debuff("Poison", 10f);
                }
                else collision.GetComponent<Enemy_Default>().TakeDamage(damage, damageForce, false, Color.green, fxType);

                offset = transform.position - collision.bounds.center;
            }
        }
        animator.SetTrigger("Hit");
        rb.velocity = Vector2.zero;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Enemy" || tag == "Boss")
        {
            // 적 충돌시 박힌채 이동
            //rb.velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
            transform.position = collision.bounds.center + offset;
        }
    }
    private void Disable_Arrow()
    {
        hit_list.Clear();
        ArrowPool.Instance.AddToPool(gameObject);
    }
}
