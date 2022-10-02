using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    
    
    float movementX;
    //public float moveForce = 10f;
    [SerializeField] [Range(100f, 2000f)] float moveSpeed = 200f;
    [SerializeField] [Range(10f, 100f)] float jumpForce = 20f;
    private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;

    private bool jump;

    private bool isGrounded;
    int playerLayer, groundLayer;

    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask WhatIsGround;
    const float GroundedRadius = 0.2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        movementX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        animator.SetFloat("Speed", Mathf.Abs(movementX));


        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        AnimatePlayer();
        //if (isGrounded) animator.SetBool("IsJumping", false);
    }

    void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundedRadius, WhatIsGround);
        for(int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
                if (!wasGrounded)
                {
                    animator.SetBool("IsJumping", false);
                }
            }
        }

        Vector3 targetVelocity = new Vector2(movementX*Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if (jump && isGrounded)
        {
            isGrounded = false;
            animator.SetBool("IsJumping", true);

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        jump = false;

        /*
        if (rb.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
        }
        else Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);*/
    }


    void AnimatePlayer()
    {
        if (movementX > 0)
        {
            sr.flipX = false;
        }
        else if (movementX < 0)
        {
            sr.flipX = true;
        }
    }
    
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (rb.velocity.y <= 0.01f && -0.01f <= rb.velocity.y)
            {
                isGrounded = true;
                animator.SetBool("IsJumping", false);
            }
        }
    }*/
}
