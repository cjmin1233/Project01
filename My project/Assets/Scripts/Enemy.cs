using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameObject player;

    public float maxHealth = 100;
    private float currentHealth;
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField] private Transform damagePoint;
    public GameObject DamageText;
    public Enemy_Healthbar healthbar;

    [HideInInspector] public bool detectPlayer;
    [HideInInspector] public bool doAttack = false;
    private float attackRate = 0.5f;
    private float lastAttackTime = 0f;
    [SerializeField] private GameObject attackCollider;
    private bool isAttacking = false;
    [Header("Horizontal Move")] [SerializeField] private float baseSpeed;
    float movementX;
    private Vector3 m_Velocity = Vector3.zero;
    private float m_MovementSmoothing = 0.05f;
    [HideInInspector] public bool canMove = true;

    [Header("Audio Source")] [SerializeField] private AudioSource die_sound;
    [SerializeField] private AudioSource[] damage_sound;

    private void Start()
    {
        movementX = baseSpeed;
        currentHealth = maxHealth;
        healthbar.SetHealth(currentHealth, maxHealth);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        detectPlayer = false;
    }
    private void FixedUpdate()
    {
        if (!animator.GetBool("IsDead"))
        {
            // 플레이어 발견. 움직임 가능한 경우
            if (detectPlayer && canMove && !doAttack && !isAttacking)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;
                EnemyMove();
            }
            if (doAttack && !isAttacking && Time.time>=lastAttackTime+1/attackRate)
            {
                EnemyAttack();
                lastAttackTime = Time.time;
            }
            if (!detectPlayer)
            {
                rb.velocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
            }
            Flip();
        }
    }
    private void EnemyMove()
    {
        Vector3 targetVelocity;

        targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        animator.SetFloat("Speed", Mathf.Abs(movementX));
    }
    private void EnemyAttack()
    {
        // 플레이어 방향으로 전환
        player = GameObject.FindGameObjectWithTag("Player");
        if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;

        animator.SetTrigger("Attack");
        isAttacking = true;
        canMove = false;
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);
    }
    private void Enable_Attack_Collider()
    {
        //attackCollider.GetComponent<Enemy_Attack_Collider>().damage = Mathf.Round(swordDamage_x * swordDamage_x_multiplier);
        //attackCollider.GetComponent<Enemy_Attack_Collider>().anim_Speed = Speed_X;
        attackCollider.SetActive(true);
    }
    private void EnemyInit()
    {
        isAttacking = false;
        canMove = true;
    }
    private void Flip()
    {
        if (movementX >= 0)
        {
            //isFacingRight = true;
            if (transform.rotation.y != 0f) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            //isFacingRight = false;
            if (transform.rotation.y == 0f) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
    public void TakeDamage(float damage, Vector2 damageForce)
    {
        int rand = Random.Range(0, 3);
        if (damage_sound[rand] != null) damage_sound[rand].PlayOneShot(damage_sound[rand].clip);
        EnemyInit();
        // 데미지 입을시 밀려나기
        //float damageForce = 20.0f;
        //if (transform.rotation.y == 0f) damageForce *= -1.0f;
        rb.AddForce(damageForce, ForceMode2D.Impulse);

        if (doAttack) canMove = false;
        animator.SetTrigger("Hit");
        GameObject dmgText = Instantiate(DamageText);
        dmgText.transform.position = damagePoint.transform.position;
        dmgText.GetComponent<DamageText>().damage = damage;
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if (die_sound != null) die_sound.PlayOneShot(die_sound.clip);
        animator.SetBool("IsDead", true);
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        GetComponent<Collider2D>().enabled = false;
        //this.enabled = false;
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        //Destroy(gameObject);
        // 골드 추가
        player = GameObject.FindGameObjectWithTag("Player");
        int random_gold = Random.Range(10, 101);
        player.GetComponent<Player>().GetGold(random_gold);
    }
    private void destroyEnemy()
    {
        Destroy(gameObject);
    }
}
