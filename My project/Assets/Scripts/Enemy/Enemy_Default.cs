using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Default : MonoBehaviour
{
    protected GameObject player;
    protected Rigidbody2D rb;
    protected Animator animator;

    // 이동 관련
    protected float currentSpeed;
    [SerializeField] protected float baseSpeed = 100f;
    [HideInInspector] public float moveSpeed_multiplier = 1f;
    protected float movementX;
    protected Vector3 m_Velocity = Vector3.zero;
    protected float m_MovementSmoothing = 0.05f;

    public bool[] range;
    [HideInInspector] public bool detectPlayer;
    protected bool canMove;

    // 공격
    protected bool isAttacking;
    [SerializeField] protected float attackRate = 0.5f;
    protected float lastAttackTime = 0f;
    private bool superArmor = false;

    // hp
    [SerializeField] protected float maxHP;
    protected float curHP;
    [SerializeField] protected Transform damagePoint;
    protected GameObject healthbar;
    [SerializeField] protected Vector3 Offset;

    // 소리
    [Header("Audio Source")] [SerializeField] private AudioSource die_sound;
    [SerializeField] protected AudioSource[] damage_sound;
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (maxHP > 0) curHP = maxHP;
        healthbar = UI_Container.Instance.GetFromEnemySliderPool();
        healthbar.GetComponent<Enemy_Healthbar>().SetHealth(curHP, maxHP);

        canMove = true;
        isAttacking = false;
        for (int i = 0; i < range.Length; i++) range[i] = false;
        currentSpeed = moveSpeed_multiplier * baseSpeed;
    }
    protected virtual void Update()
    {
        // 체력바 이동
        healthbar.GetComponent<Slider>().transform.position = Camera.main.WorldToScreenPoint(transform.position + Offset);
        if (!animator.GetBool("IsDead"))
        {
            if (!detectPlayer) canMove = false;
            else
            {
                for (int i = 0; i < range.Length; i++)
                {
                    if (range[i])
                    {
                        canMove = false;
                        if (Time.time >= lastAttackTime + 1 / attackRate && !isAttacking)
                        {
                            LookPlayer();
                            Attack(i);
                            lastAttackTime = Time.time;
                        }
                        break;
                    }
                }
            }

            currentSpeed = moveSpeed_multiplier * baseSpeed;
            if (canMove)
            {
                movementX = currentSpeed;
                LookPlayer();
            }
            else
            {
                movementX = 0f;
            }
            animator.SetFloat("Speed", Mathf.Abs(movementX));

            Flip();
        }
    }
    protected virtual void FixedUpdate()
    {
        Move();
    }
    private void Init()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", isAttacking);
        canMove = true;
        superArmor = false;
    }
    protected virtual void LookPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;
    }
    protected void Flip()
    {
        if (movementX > 0 && transform.rotation.y != 0f) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (movementX < 0 && transform.rotation.y == 0f) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
    protected virtual void Move()
    {
        Vector3 targetVelocity;

        targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }
    protected virtual void Attack(int idx)
    {
        animator.SetTrigger("Attack" + idx);
        isAttacking = true;
        animator.SetBool("IsAttacking", isAttacking);
    }
    private void OnSuperArmor()
    {
        superArmor = true;
    }
    private void OffSuperArmor()
    {
        superArmor = false;
    }

    public virtual void TakeDamage(float damage, Vector2 damageForce)
    {
        if (!animator.GetBool("IsDead"))
        {
            if (damage_sound.Length > 0)
            {
                int rand = Random.Range(0, damage_sound.Length);
                if (damage_sound[rand] != null) damage_sound[rand].PlayOneShot(damage_sound[rand].clip);
            }

            // 수퍼아머가 아니라면 밀려남
            if (!superArmor) rb.AddForce(damageForce, ForceMode2D.Impulse);
            // 밀치는 힘이 있다면 타격 애니메이션
            if (damageForce.x != 0f) animator.SetTrigger("Hit");

            GameObject dmgText = DamageTextPool.Instance.GetFromPool();
            dmgText.transform.position = damagePoint.transform.position;
            dmgText.GetComponent<DamageText>().damage = damage;
            dmgText.SetActive(true);
            curHP -= damage;
            healthbar.GetComponent<Enemy_Healthbar>().SetHealth(curHP, maxHP);
            if (curHP <= 0)
            {
                curHP = 0;
                Die();
            }
        }
    }
    protected virtual void Die()
    {
        if (die_sound != null) die_sound.PlayOneShot(die_sound.clip);
        animator.SetBool("IsDead", true);
        canMove = false;

        // 골드 생성
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Player>().GetGold(100);

        // 체력바 반납
        UI_Container.Instance.AddToEnemySliderPool(healthbar);
    }
    private void BackStep()
    {
        Vector2 force = new Vector2(transform.right.x * (-20f), 0f);
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    private void Dash_1()
    {
        Vector2 force = new Vector2(transform.right.x * 15f, 0f);
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    private void Dash_2()
    {
        Vector2 force = new Vector2(transform.right.x * 30f, 0f);
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    private void Dash_3()
    {
        Vector2 force = new Vector2(transform.right.x * 60f, 0f);
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    private void ThrowBomb()
    {
        GameObject gameObject = EnemyBulletPool.Instance.GetFromPool();
        gameObject.GetComponent<Enemy_Bullet>().damage = 10f;
        gameObject.GetComponent<Enemy_Bullet>().type = 1;
        gameObject.transform.position = transform.position + new Vector3(0.1f, 0.1f, 0f);
        gameObject.transform.rotation = transform.rotation;
        gameObject.SetActive(true);
    }
    private void destoryObject()
    {
        Destroy(gameObject);
    }
}
