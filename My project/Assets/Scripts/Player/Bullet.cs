using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float anim_Speed;
    [HideInInspector] public bool isPoisoned;
    [HideInInspector] public bool isDiagonal;
    private float speed = 25f;
    //public GameObject ImpactEffect;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private new BoxCollider2D collider;
    [SerializeField] private BoxCollider2D collider_diagonal;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim_Speed = 1.0f;
        isPoisoned = false;
        isDiagonal = false;
        collider.enabled = true;
    }
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //transform.position = player.position;
        transform.rotation = player.rotation;
        animator.SetBool("IsPoisoned", isPoisoned);
        animator.SetBool("IsDiagonal", isDiagonal);
        //animator.SetFloat("EnableSpeed", anim_Speed);
        if (isDiagonal) rb.velocity = new Vector2(transform.right.x * speed, transform.up.y * (-speed));
        else rb.velocity = transform.right * speed;
        /*if(isDiagonal) transform.rotation = Quaternion.Euler(0f, 0f, -45f);
        else transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rb.velocity = transform.right * speed;*/
        if (isDiagonal)
        {
            collider.enabled = false;
            collider_diagonal.enabled = true;
        }
        else
        {
            collider.enabled = true;
            collider_diagonal.enabled = false;
        }
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
            // �� �浹�� ����ä �̵�
            rb.velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
        }
    }
    private void Disable_Arrow()
    {
        ArrowPool.Instance.AddToPool(gameObject);
    }
}
