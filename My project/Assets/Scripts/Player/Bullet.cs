using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int fxType;
    [HideInInspector] public float damage;
    //[HideInInspector] public float anim_Speed;
    [HideInInspector] public bool isPoisoned;
    [HideInInspector] public bool isDiagonal;
    private float speed = 25f;
    //public GameObject ImpactEffect;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D arrow_collider;
    private Vector2 offset;
    private Vector2 offset_diagonal;
    private List<string> hit_list;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        arrow_collider = GetComponent<BoxCollider2D>();
        //anim_Speed = 1.0f;
        isPoisoned = false;
        isDiagonal = false;
        offset = new Vector2(0.11f, 0f);
        offset_diagonal = new Vector2(0.08f, -0.08f);
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
        if (isDiagonal)  arrow_collider.offset = offset_diagonal;
        else arrow_collider.offset = offset;
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
                collision.GetComponent<Enemy_Default>().TakeDamage(damage, Vector2.zero, fxType);
                if (isPoisoned) collision.GetComponent<Enemy_Default>().TakeDamage(1f, Vector2.zero, fxType + 1);
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
            rb.velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
        }
    }
    private void Disable_Arrow()
    {
        hit_list.Clear();
        ArrowPool.Instance.AddToPool(gameObject);
    }
}
