using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    
    //public float moveForce = 10f;
    [Header("Horizontal Movement")]
    float movementX;
    [SerializeField] [Range(100f, 2000f)] private float baseSpeed = 400f;
    private float currentSpeed;
    [SerializeField] [Range(10f, 100f)] private float jumpForce = 10f;
    private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;
    public float dashPower = 5f;
    public float dashTime = 0.2f;
    private bool isDashing = false;
    public float distanceBetweenImages;
    public float dashCoolDown;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;
    private bool isFacingRight = true;

    [Header("Vertical Movement")]
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
    private void Start()
    {
        currentSpeed = baseSpeed;
        int weaponType = PlayerPrefs.GetInt("weaponType");
        animator.SetInteger("WeaponType", weaponType);
    }

    void Update()
    {
        if (!isDashing) movementX = Input.GetAxisRaw("Horizontal") * currentSpeed;
        animator.SetFloat("Speed", Mathf.Abs(movementX));


        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        FlipPlayer();

        
        if (Input.GetButtonDown("Dash"))
        {
            if (!isDashing && !animator.GetBool("IsJumping"))
            {
                if (Time.time >= (lastDash + dashCoolDown))
                {
                    AttemptToDash();
                }
                //StartCoroutine(Dash());
            }
        }
        CheckDash();
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

        Vector3 targetVelocity;
        if (isDashing)
        {
            targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, 0f);
        }
        else targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, rb.velocity.y);

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


    void FlipPlayer()
    {
        if (movementX > 0)
        {
            isFacingRight = true;
            if (transform.rotation.y != 0f) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (movementX < 0)
        {
            isFacingRight = false;
            if (transform.rotation.y == 0f) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        currentSpeed *= dashPower;
        if (!sr.flipX) movementX = currentSpeed;
        else if (sr.flipX) movementX = (-1f) * currentSpeed;


        yield return new WaitForSeconds(dashTime);

        currentSpeed = baseSpeed;
        isDashing = false;
    }
    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        AfterimagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }
    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                currentSpeed = dashPower * baseSpeed;
                if (isFacingRight) movementX = currentSpeed;
                else movementX = (-1f) * currentSpeed;

                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    AfterimagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }

            }
            if (dashTimeLeft <= 0)
            {
                currentSpeed = baseSpeed;
                isDashing = false;
            }
        }
    }
}
