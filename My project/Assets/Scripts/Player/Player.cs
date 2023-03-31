using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private PlayerAttack playerAttack;
    
    float movementX;
    [Header("Horizontal Movement")] private float baseSpeed = 400f;
    [HideInInspector] public float moveSpeed_multiplier = 1f;
    public AudioSource[] dash_sound;
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

    // after image 
    public bool AfterImageAvailable = false;
    private float AfterImageRate = 20f;
    private float lastAfterImageTime = -100f;

    [Header("Vertical Movement")]
    public AudioSource[] jump_sound;
    private bool jump;
    private bool isGrounded;
    private float prev_vel_y;

    [SerializeField] private LayerMask WhatIsGround;

    [HideInInspector] public float defence_multiplier = 1f;
    [HideInInspector] public float hpincrease_multiplier = 1f;
    [HideInInspector] public bool recovery_enable = false;
    [HideInInspector] public bool resistance_enable = false;
    [HideInInspector] public bool dodge_enable = false;
    [HideInInspector] public bool guard_enable = false;
    private float lastDamageTime = -100f;

    [HideInInspector] public float MaxHP;
    [HideInInspector] public float CurHP;
    [HideInInspector] public bool canInvincible;
    [SerializeField] private AudioSource damage_shield_sound;
    private float damagingTimeLeft = -1f;
    const float damagingTime = 1f;

    [HideInInspector] public float gold_multiplier = 1f;
    private int gold = 0;
    [SerializeField] private Player_Ground_Checker ground_checker;

    [HideInInspector] public bool isDead = false;
    private void OnEnable()
    {
        canInvincible = false;
        damagingTimeLeft = -1f;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();

        if (!GameManager.Instance.newGame)
        {
            Data gameData = DataManager.Instance.data;
            MaxHP = gameData.maxHP;
            CurHP = gameData.curHP;
            Debug.Log("hp is : " + MaxHP.ToString() + ", " + CurHP.ToString());
            gold = gameData.gold;
        }
        else
        {
            MaxHP = 100f;
            CurHP = MaxHP;
            gold = 0;
        }
        currentSpeed = moveSpeed_multiplier * baseSpeed;
    }

    private void Update()
    {
        if (!isDead)
        {
            //
            if (Time.time > lastDamageTime + 20f && guard_enable)
            {
                UI_Container.Instance.AddPlayerBuff("Guard", -1f);
            }
            //
            if (resistance_enable && CurHP / MaxHP <= 0.4f)
            {
                UI_Container.Instance.AddPlayerBuff("Resistance", -1f);
            }
            else
            {
                UI_Container.Instance.AddPlayerBuff("Resistance", 0f);
            }

            // HP bar test.
            if (Input.GetKeyDown(KeyCode.A))    //Increase hp.
            {
                IncreaseMaxHP();
            }
            if (Input.GetKeyDown(KeyCode.Q))    //Take damage.
            {
                TakeDamage(20f);
            }
            /////////////////////
            currentSpeed = moveSpeed_multiplier * baseSpeed;
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


            if (Input.GetButton("Jump") && !isDashing && !animator.GetBool("IsJumping"))
            {
                jump = true;
            }

        
            if (Input.GetButtonDown("Dash"))
            {
                if (!isDashing && !animator.GetBool("IsJumping"))
                {
                    if (Time.time >= (lastDash + dashCoolDown))
                    {
                        movementX = Input.GetAxisRaw("Horizontal");
                        AttemptToDash();
                    }
                }
            }
            //DamagingCheck();
            FlipPlayer();
            CheckDash();
            CheckAfterImage();
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            HorizontalMove();
            VerticalMove();
        }
    }
    private void FlipPlayer()
    {
        if (movementX > 0)
        {
            if (transform.rotation.y != 0f) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (movementX < 0)
        {
            if (transform.rotation.y == 0f) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
    private void AttemptToDash()
    {
        playerAttack.PlayerInit();
        // 대쉬 효과음
        int rand = Random.Range(0, 2);
        if (dash_sound[rand]!=null) dash_sound[rand].PlayOneShot(dash_sound[rand].clip);

        // 대쉬 조건 초기화
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
                // 대쉬 방향 설정
                animator.SetBool("IsDashing", isDashing);
                currentSpeed = dashPower * moveSpeed_multiplier * baseSpeed;
                movementX = transform.right.x * currentSpeed;


                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    AfterimagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }

            }
            if (dashTimeLeft <= 0)
            {
                // 대쉬 정지
                canMove = true;
                currentSpeed = moveSpeed_multiplier * baseSpeed;
                isDashing = false;
                animator.SetBool("IsDashing", isDashing);
                if (damagingTimeLeft <= 0)
                {
                    // 데미지를 입은 상태가 아니라면 무적 해제
                    Invincible_OFF();
                }
            }
        }
    }
    private void CheckAfterImage()
    {
        if (AfterImageAvailable && !isDashing)
        {
            if (Time.time > lastAfterImageTime + 1 / AfterImageRate)
            {
                AfterimagePool.Instance.GetFromPool();
                lastAfterImageTime = Time.time;
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
        ///////////////////////////////
        bool wasGrounded = isGrounded;
        isGrounded = false;
        if (GetComponent<Player_Collider_Checker>().groundCollision && ground_checker.isGrounded) isGrounded = true;
        if (!wasGrounded && isGrounded)
        {
            // 공중에서 착지
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsJumpingDown", false);
            //animator.SetBool("IsFalling", false);
            //rb.velocity = Vector2.zero;
        }
        //////////////////////////////
        /*bool wasGrounded = isGrounded;
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundedRadius, WhatIsGround);

        //if (colliders.Length == 0 && !wasGrounded) Debug.Log("I'm flying!");
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
                //Debug.Log(colliders[i].gameObject.layer);
                if (!wasGrounded)
                {
                    // 현재는 땅에 닿았으나 이전 프레임에는 닿지 않은 경우, 즉 방금 착지한 경우
                    animator.SetBool("IsJumping", false);
                    animator.SetBool("IsJumpingDown", false);
                }
            }
        }*/
        /*// 점프하지 않고 낙하하는 경우
        if (wasGrounded && !isGrounded && rb.velocity.y <= 0)
        {
            Debug.Log("i'm falling! Am I jumping? : " + animator.GetBool("IsJumping"));
        }*/
        // 점프없이 낙하하는 경우
        if (wasGrounded && !isGrounded)
        {
            Debug.Log("Not grounded");
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsJumpingDown", true);
            animator.SetTrigger("Fall");
        }
        if (prev_vel_y > 0 && rb.velocity.y <= 0 && animator.GetBool("IsJumping"))
        {
            // 점프중 낙하구간
            //Debug.Log("Jumping and falling");
            animator.SetBool("IsJumpingDown", true);
        }
        prev_vel_y = rb.velocity.y;
        if (jump && isGrounded && canMove)
        {
            isGrounded = false;
            animator.SetBool("IsJumping", true);
            playerAttack.PlayerInit();

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            int rand = Random.Range(0, 3);
            if (jump_sound[rand]!=null) jump_sound[rand].PlayOneShot(jump_sound[rand].clip);
        }
        jump = false;
    }
    public void IncreaseMaxHP()
    {
        MaxHP += Mathf.Round(25f * hpincrease_multiplier);
        CurHP += Mathf.Round(25f * hpincrease_multiplier);

        UI_Container.Instance.HandleHP(CurHP, MaxHP);
    }
    public void Heal(float heal)
    {
        CurHP += heal;
        if (CurHP > MaxHP) CurHP = MaxHP;
        UI_Container.Instance.HandleHP(CurHP, MaxHP);
    }
    public void TakeDamage(float damage)
    {
        // 죽지 않았고 무적이 아닐 때
        if (!canInvincible && !isDead)
        {
            if (isDashing)
            {
                // 피격 회피
                UI_Container.Instance.EnableEventText("Dodge", "회피!");
                if (dodge_enable) UI_Container.Instance.AddPlayerBuff("Dodge", 3f);
            }
            else
            {
                // 데미지 무시가능한 경우
                if (guard_enable && Time.time > lastDamageTime + 20f)
                {
                    Debug.Log("데미지 무시!");
                    UI_Container.Instance.AddPlayerBuff("Guard", 0f);
                }
                else
                {
                    if (!playerAttack.isZAttacking && !playerAttack.isXAttacking && !animator.GetBool("IsJumping") && movementX == 0f) animator.SetTrigger("Hit");
                    // 플레이어 밀침 효과
                    float damageForce = 20.0f;
                    rb.AddForce(new Vector2(-1f * transform.right.x * damageForce, 0f), ForceMode2D.Impulse);
                    damage /= defence_multiplier;

                    if (playerAttack.sword_shield_enable && playerAttack.isXAttacking)
                    {
                        damage *= 0.5f;
                        if (damage_shield_sound != null) damage_shield_sound.PlayOneShot(damage_shield_sound.clip);
                    }

                    if (resistance_enable && CurHP / MaxHP <= 0.4f) damage *= 0.9f;

                    CurHP -= Mathf.Round(damage);
                    // 무적 시간 부여
                    canInvincible = true;
                    damagingTimeLeft = damagingTime;
                    StartCoroutine(Damaging_Check());
                    if (CurHP <= 0 && !isDead)
                    {
                        // 플레이어 사망
                        Die();
                    }
                    else if (recovery_enable)
                    {
                        damage *= 0.3f;
                        Heal(damage);
                    }
                }
                lastDamageTime = Time.time;
            }
        }
        UI_Container.Instance.HandleHP(CurHP, MaxHP);
    }
    public void GetGold(int get_gold)
    {
        get_gold = (int)Mathf.Round(get_gold * gold_multiplier);
        gold += get_gold;
        UI_Container.Instance.HandleGold(gold);
    }
    public int CheckGold()
    {
        return gold;
    }
    public void Purchase(int price)
    {
        gold -= price;
        UI_Container.Instance.HandleGold(gold);
    }
    private IEnumerator Damaging_Check()
    {
        float counter = 0;
        Color c = Color.white;
        while (damagingTimeLeft > 0f)
        {
            damagingTimeLeft -= Time.deltaTime;
            counter += Time.deltaTime;
            if (counter >= 0.125f)
            {
                counter = 0f;
                if (sr.color.a > 0f) c.a = 0f;
                else c.a = 1f;
                sr.color = c;
            }

            yield return null;
        }
        canInvincible = false;
        sr.color = Color.white;
    }
    public void Invincible_ON()
    {
        canInvincible = true;
    }
    public void Invincible_OFF()
    {
        canInvincible = false;
    }
    private void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", isDead);

        CurHP = 0;
        // 사망 소리
        canMove = false;
        //rb.gravityScale = 0f;
        //rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;

        // 사망 UI 출력
        UI_Container.Instance.EnableDieUI();

        GameManager.Instance.isPlaying = false;
    }
}
