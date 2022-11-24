using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    
    //public float moveForce = 10f;
    float movementX;
    [Header("Horizontal Movement")][SerializeField] private float baseSpeed = 400f;
    public float dashPower = 5f;
    public float dashTime = 0.2f;
    public float distanceBetweenImages;
    public float dashCoolDown;
    [HideInInspector] public bool canMove=true;
    [SerializeField] private float jumpForce = 10f;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_MovementSmoothing = .05f;
    private bool isDashing = false;
    private float currentSpeed;
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
    const float GroundedRadius = 0.1f;

    [SerializeField] private GameObject HP_Bar;
    [HideInInspector] public int MaxHP = 100;
    [HideInInspector] public int CurHP;
    private bool canInvincible = false;
    float invincibleTimeLeft;
    float invincibleTime = 1f;
    [SerializeField] private GameObject Esc_UI;
    [SerializeField] private GameObject Book_UI;
    private void Start()
    {
        invincibleTimeLeft = 0f;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");

        CurHP = MaxHP;
        HP_Bar.gameObject.GetComponent<Player_HP_Manager>().HandleHP();
        currentSpeed = baseSpeed;
        int weaponType = PlayerPrefs.GetInt("weaponType");
        animator.SetInteger("WeaponType", weaponType);
    }

    private void Update()
    {
        // HP bar test.
        if (Input.GetKeyDown(KeyCode.A))    //Increase hp.
        {
            IncreaseMaxHP();
        }
        if (Input.GetKeyDown(KeyCode.Q))    //Take damage.
        {
            TakeDamage(20);
        }
        /////////////////////
        if (canMove)
        {
            if (!isDashing) movementX = Input.GetAxisRaw("Horizontal") * currentSpeed;
            animator.SetFloat("Speed", Mathf.Abs(movementX));
        }
        else
        {
            if (!isDashing) movementX = 0f;
            animator.SetFloat("Speed", Mathf.Abs(movementX));
        }


        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        
        if (Input.GetButtonDown("Dash"))
        {
            if (!isDashing && !animator.GetBool("IsJumping"))
            {
                if (Time.time >= (lastDash + dashCoolDown))
                {
                    AttemptToDash();
                }
            }
        }

        // PopUp UI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Esc_UI.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            //
            Book_UI.SetActive(true);
        }
        Invincible();
        FlipPlayer();
        CheckDash();
    }

    private void FixedUpdate()
    {
        HorizontalMove();
        VerticalMove();
    }


    private void FlipPlayer()
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
    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        canInvincible = true;
        if (invincibleTimeLeft < dashTimeLeft)
        {
            invincibleTimeLeft = dashTimeLeft;
        }

        AfterimagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }
    private void CheckDash()
    {
        if (isDashing)
        {
            gameObject.GetComponent<PlayerAttack>().isZAttacking = false;
            gameObject.GetComponent<PlayerAttack>().isXAttacking = false;
            gameObject.GetComponent<PlayerAttack>().comboCounter = 0;
            if (dashTimeLeft > 0)
            {
                animator.SetBool("IsDashing", isDashing);
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
                canMove = true;
                currentSpeed = baseSpeed;
                isDashing = false;
                animator.SetBool("IsDashing", isDashing);
            }
        }
    }
    private void HorizontalMove()
    {
        Vector3 targetVelocity;
        if (isDashing)
        {
            targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, 0f);
        }
        else targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, rb.velocity.y);

        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }
    private void VerticalMove()
    {
        //////////////////////////////
        bool wasGrounded = isGrounded;
        isGrounded = false;
        //if(isStuckable) Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundedRadius, WhatIsGround);
        if (colliders.Length == 0 && !wasGrounded) Debug.Log("I'm flying!");
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
                if (!wasGrounded)
                {
                    animator.SetBool("IsJumping", false);
                    //Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
                    //Debug.Log("I'm grounded");
                }
            }
        }
        if (jump && isGrounded && canMove)
        {
            isGrounded = false;
            animator.SetBool("IsJumping", true);
            gameObject.GetComponent<PlayerAttack>().isZAttacking = false;
            gameObject.GetComponent<PlayerAttack>().isXAttacking = false;
            gameObject.GetComponent<PlayerAttack>().comboCounter = 0;


            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        jump = false;
        /*
        if (!ignoreGround)
        {
            //////////////////////////////
            bool wasGrounded = isGrounded;
            isGrounded = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundedRadius, WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    isGrounded = true;
                    if (!wasGrounded)
                    {
                        animator.SetBool("IsJumping", false);
                        Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
                    }
                }
            }
            if (jump && isGrounded)
            {
                isGrounded = false;
                animator.SetBool("IsJumping", true);
                gameObject.GetComponent<PlayerAttack>().isZAttacking = false;
                gameObject.GetComponent<PlayerAttack>().isXAttacking = false;
                gameObject.GetComponent<PlayerAttack>().comboCounter = 0;


                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            }
            jump = false;

        }

        
        bool wasGrounded = isGrounded;
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundedRadius, WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
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
        if (jump && isGrounded)
        {
            isGrounded = false;
            animator.SetBool("IsJumping", true);
            gameObject.GetComponent<PlayerAttack>().isZAttacking = false;
            gameObject.GetComponent<PlayerAttack>().isXAttacking = false;
            gameObject.GetComponent<PlayerAttack>().comboCounter = 0;


            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        jump = false;
        */
        //if (ignoreGround) Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
        //else Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);


    }
    public void IncreaseMaxHP()
    {
        MaxHP += 25;
        CurHP += 25;
        HP_Bar.gameObject.GetComponent<Player_HP_Manager>().HandleHP();
    }
    public void TakeDamage(int damage)
    {
        if (!canInvincible)
        {
            float damageForce = 20.0f;
            if (isFacingRight) damageForce *= -1.0f;
            rb.AddForce(new Vector2(damageForce, 5f), ForceMode2D.Impulse);
            CurHP -= damage;
            canInvincible = true;
            invincibleTimeLeft = invincibleTime;
            Color invincibleColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            sr.color = invincibleColor;

            if (CurHP <= 0)
            {
                CurHP = 0;
                Die();
            }
        }

        HP_Bar.gameObject.GetComponent<Player_HP_Manager>().HandleHP();
    }
    private void Invincible()
    {
        if (canInvincible)
        {
            invincibleTimeLeft -= Time.deltaTime;
            if (invincibleTimeLeft <= 0)
            {
                canInvincible = false;
                sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }
    /*
    public void EnableIgnoreGround()
    {
        ignoreGround = true;
        Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
    }
    public void DisableIgnoreGround()
    {
        ignoreGround = false;
        Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
    }
     */
    private void Die()
    {
        //animator.SetBool("IsDead", true);
        //GetComponent<Rigidbody2D>().gravityScale = 0;
        //GetComponent<Collider2D>().enabled = false;
        Debug.Log("Player Died");
        //this.enabled = false;
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        //Destroy(gameObject);
    }

    // UI part.
    /*
    public void TimeStop()
    {
        Time.timeScale = 0;
    }
    public void TimeStart()
    {
        Time.timeScale = 1;
    }*/
}
