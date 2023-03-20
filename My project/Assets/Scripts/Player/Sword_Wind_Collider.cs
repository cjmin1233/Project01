using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Wind_Collider : MonoBehaviour
{
    public int fxType;
    [HideInInspector] public float damage;
    [HideInInspector] public Vector2 damageForce;
    [HideInInspector] public float anim_Speed;
    private Transform player;

    private Animator animator;
    private Rigidbody2D rb;
    private List<string> hit_list;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        hit_list = new List<string>();
        anim_Speed = 1.0f;
    }

    private void OnEnable()
    {
        //
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.rotation = player.rotation;
        //
        animator.SetFloat("EnableSpeed", anim_Speed);

        rb.velocity = 10f * anim_Speed * transform.right;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;
        if (!hit_list.Contains(name))
        {
            hit_list.Add(name);
            if (tag == "Enemy" || tag == "Boss")
            {
                collision.GetComponent<Enemy_Default>().TakeDamage(damage, damageForce, false, Color.red, fxType);
            }
        }
    }
    private void Disable_Sword_Collider()
    {
        hit_list.Clear();
        SwordWindPool.Instance.AddToPool(gameObject);
    }
}
