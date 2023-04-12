using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Shower_Startpoint : MonoBehaviour
{
    [HideInInspector] public float anim_Speed;

    private Rigidbody2D rb;
    private float speed = 20f;
    Vector3 offset;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        offset = transform.localPosition;
        anim_Speed = 1f;
    }
    private void OnEnable()
    {
        transform.localPosition = offset;
        rb.velocity = anim_Speed * speed * transform.right;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Enemy" || tag == "Boss")
        {
            rb.velocity = Vector2.zero;
        }
    }
}
