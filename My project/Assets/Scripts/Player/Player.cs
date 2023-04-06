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
    [HideInInspector] public int invincibleCounter;
    [HideInInspector] public bool canInvincible;
    [HideInInspector] public bool damagingInvincible;
    [HideInInspector] public bool skillInvincible;
    [SerializeField] private AudioSource damage_shield_sound;
    private float damagingTimeLeft;
    const float damagingTime = 1f;

    [HideInInspector] public float gold_multiplier = 1f;
    private int gold = 0;
    [SerializeField] private Player_Ground_Checker ground_checker;

    [HideInInspector] public bool isDead = false;
    private void OnEnable()
    {
        canInvincible = false;
        damagingInvincible = false;
        skillInvincible = false;
        invincibleCounter = 0;
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
            // 20�ʵ��� �ȸ����� ���� �����Ƽ Ȱ��ȭ
            if (Time.time > lastDamageTime + 20f && guard_enable)
            {
                UI_Container.Instance.AddPlayerBuff("Guard", -1f);
            }
            // ü�� 40�� ���Ͻ� ������ ���� �����Ƽ Ȱ��ȭ
            if (resistance_enable && CurHP / MaxHP <= 0.4f)
            {
                UI_Container.Instance.AddPlayerBuff("Resistance", -1f);
            }
            else
            {
                UI_Container.Instance.AddPlayerBuff("Resistance", 0f);
            }

            /////////////////////
            // Testing code
            if (Input.GetKeyDown(KeyCode.A))
            {
                // Increse hp.
                IncreaseMaxHP();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // Take damage.
                TakeDamage(20f);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                // Get ability.
                UI_Container.Instance.GetRandomAbility();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                // Upgrade ability.
                UI_Container.Instance.UpgradeRandomAbility();
            }
            /////////////////////
            if (!isDashing)
            {
                currentSpeed = moveSpeed_multiplier * baseSpeed;
                if (canMove) movementX = Input.GetAxisRaw("Horizontal") * currentSpeed;
                else movementX = 0f;
            }
            animator.SetFloat("Speed", Mathf.Abs(movementX));

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
                        jump = false;   // ������ �����Է� ����
                        movementX = Input.GetAxisRaw("Horizontal");
                        AttemptToDash();
                    }
                }
            }
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
        // �÷��̾� transform.rotation ������ �Լ�

        // ���������� �����µ� ������ ���� �ִ� ���
        if (movementX > 0 && transform.rotation.y != 0f) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        // �������� �����µ� �������� ���� �ִ� ���
        else if (movementX < 0 && transform.rotation.y == 0f) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
    private void AttemptToDash()
    {
        playerAttack.PlayerInit();
        // �뽬 ȿ����
        int rand = Random.Range(0, 2);
        if (dash_sound[rand]!=null) dash_sound[rand].PlayOneShot(dash_sound[rand].clip);

        // �뽬 ���� �ʱ�ȭ
        isDashing = true;
        animator.SetBool("IsDashing", isDashing);
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        currentSpeed = dashPower * moveSpeed_multiplier * baseSpeed;

        AfterimagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }
    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                // �뽬 ���� ����
                movementX = transform.right.x * currentSpeed;
                dashTimeLeft -= Time.deltaTime;

                // �ܻ�� ���� ���� �̻��� �� �ܻ� ����
                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    AfterimagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }
            if (dashTimeLeft <= 0)
            {
                // �뽬 ����
                isDashing = false;
                animator.SetBool("IsDashing", isDashing);
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
        bool wasGrounded = isGrounded;
        isGrounded = false;
        if (GetComponent<Player_Collider_Checker>().groundCollision && ground_checker.isGrounded) isGrounded = true;
        if (!wasGrounded && isGrounded)
        {
            // ���߿��� ����
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsJumpingDown", false);
        }
        // �������� �����ϴ� ���
        if (wasGrounded && !isGrounded)
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsJumpingDown", true);
            animator.SetTrigger("Fall");
        }
        if (prev_vel_y > 0 && rb.velocity.y <= 0 && animator.GetBool("IsJumping"))
        {
            // ������ ���ϱ���
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
        float increase = Mathf.Round(25f * hpincrease_multiplier);
        MaxHP += increase;
        CurHP += increase;

        UI_Container.Instance.HandleHP(CurHP, MaxHP);
        UI_Container.Instance.EnableEventText("Heal", "+" + ((int)increase).ToString() + "HP");
    }
    public void Heal(float heal)
    {
        CurHP += heal;
        if (CurHP > MaxHP) CurHP = MaxHP;
        UI_Container.Instance.HandleHP(CurHP, MaxHP);
        UI_Container.Instance.EnableEventText("Heal", "+" + ((int)heal).ToString());
    }
    public void TakeDamage(float damage)
    {
        // ���� �ʾҰ� ������ �ƴ� ��, ȭ�� ��ȯ���� �ƴ� ��
        if (!isDead && !damagingInvincible && !skillInvincible && GameManager.Instance.fadeState=="clear")
        {
            if (isDashing)
            {
                // �뽬 �� ��� �ǰ� ����
                UI_Container.Instance.EnableEventText("Dodge", "ȸ��!");
                if (dodge_enable) UI_Container.Instance.AddPlayerBuff("Dodge", 3f);
            }
            else
            {
                // ������ ���ð����� ���
                if (guard_enable && Time.time > lastDamageTime + 20f)
                {
                    UI_Container.Instance.AddPlayerBuff("Guard", 0f);
                }
                else
                {
                    // �÷��̾� ��ħ ȿ��
                    float damageForce = 20.0f;
                    rb.AddForce(new Vector2(-1f * transform.right.x * damageForce, 0f), ForceMode2D.Impulse);

                    #region ������ ���
                    damage /= defence_multiplier;

                    if (playerAttack.sword_shield_enable && playerAttack.isXAttacking)
                    {
                        damage *= 0.5f;
                        if (damage_shield_sound != null) damage_shield_sound.PlayOneShot(damage_shield_sound.clip);
                    }

                    if (resistance_enable && CurHP / MaxHP <= 0.4f) damage *= 0.9f;
                    #endregion

                    CurHP -= Mathf.Round(damage);
                    // ������ ǥ��
                    if (damage > 0f)
                    {
                        UI_Container.Instance.EnableEventText("Damage", "-" + Mathf.RoundToInt(damage).ToString());
                        // �ǰ� �ִϸ��̼�
                        if (!playerAttack.isZAttacking && !playerAttack.isXAttacking && !animator.GetBool("IsJumping") && movementX == 0f)
                        {
                            animator.SetTrigger("Hit");
                        }
                    }
                    // ���� �ð� �ο�
                    StartCoroutine(Damaging_Check());
                    if (CurHP <= 0f && !isDead)
                    {
                        // �÷��̾� ���
                        Die();
                    }
                    else if (recovery_enable)
                    {
                        damage *= 0.3f;
                        Heal(damage);
                    }
                }
                // ������ �ǰ� �ð� ǥ��
                lastDamageTime = Time.time;
            }
        }
        UI_Container.Instance.HandleHP(CurHP, MaxHP);
    }
    public void GetGold(int get_gold)
    {
        get_gold = (int)Mathf.Round(get_gold * gold_multiplier);
        gold += get_gold;
        if (get_gold > 0)
        {
            UI_Container.Instance.HandleGold(gold);
            UI_Container.Instance.EnableEventText("Gold", "+" + get_gold.ToString() + "G");
        }
    }
    public int CheckGold()
    {
        return gold;
    }
    public void Purchase(int price)
    {
        gold -= price;
        if (price > 0)
        {
            UI_Container.Instance.HandleGold(gold);
            UI_Container.Instance.EnableEventText("Purchase", "-" + price.ToString() + "G");
        }
    }
    private IEnumerator Damaging_Check()
    {
        float timer = 0;
        Color damagingColor = Color.white;
        damagingTimeLeft = damagingTime;
        damagingInvincible = true;
        while (damagingTimeLeft > 0f)
        {
            damagingTimeLeft -= Time.deltaTime;
            timer += Time.deltaTime;
            // 0.125�ʸ��� ��������Ʈ ����
            if (timer >= 0.125f)
            {
                timer = 0f;
                if (sr.color.a > 0f) damagingColor.a = 0f;
                else damagingColor.a = 1f;
                sr.color = damagingColor;
            }

            yield return null;
        }
        damagingInvincible = false;
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
        // ��� �Ҹ�
        canMove = false;
        //rb.gravityScale = 0f;
        //rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;

        // ��� UI ���
        UI_Container.Instance.EnableDieUI();

        // ����Ʈ �����Ƽ ��Ȱ��ȭ
        var surpent = transform.Find("Serpent_Screw")?.gameObject;
        if (surpent != null) surpent.SetActive(false);
        GameManager.Instance.isPlaying = false;
    }
}
