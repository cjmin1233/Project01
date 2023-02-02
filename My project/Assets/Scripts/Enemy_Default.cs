using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Default : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;

    // 이동 관련
    [SerializeField] private float baseSpeed = 100f;
    private float movementX;
    private Vector3 m_Velocity = Vector3.zero;
    private float m_MovementSmoothing = 0.05f;

    [HideInInspector] public bool playerInRange;
    [HideInInspector] public bool detectPlayer;
    //private bool canMove;

    // 공격
    private bool isAttacking;
    [SerializeField] private float attackRate = 0.5f;
    private float lastAttackTime = 0f;
    private bool superArmor = false;

    // hp
    [SerializeField] private float maxHP;
    private float curHP;
    [SerializeField] private Transform damagePoint;
    //[SerializeField] private GameObject DamageText;
    //[SerializeField] private Enemy_Healthbar healthbar;
    private GameObject healthbar;
    [SerializeField] private Vector3 Offset;

    // 소리
    [Header("Audio Source")] [SerializeField] private AudioSource die_sound;
    [SerializeField] private AudioSource[] damage_sound;
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (maxHP > 0) curHP = maxHP;
        healthbar = UI_Container.Instance.GetFromEnemySliderPool();
        //healthbar.GetComponent<Enemy_Healthbar>().enemyTransform = transform;
        healthbar.GetComponent<Enemy_Healthbar>().SetHealth(curHP, maxHP);
        //healthbar.SetHealth(curHP, maxHP);

        //canMove = true;
        isAttacking = false;
        playerInRange = false;
    }
    protected virtual void FixedUpdate()
    {
        // 체력바 이동
        healthbar.GetComponent<Slider>().transform.position = Camera.main.WorldToScreenPoint(transform.position + Offset);
        if (!isAttacking && !animator.GetBool("IsDead"))
        {
            if (!detectPlayer)
            {
                // 플레이어를 발견하지 못한 경우 행동하지 않는다.
                Stop();
            }
            else if (!playerInRange)
            {
                // 플레이어를 발견했고 공격범위 안에 없는 경우 플레이어를 추격한다.
                //canMove = true;
                //
                movementX = baseSpeed;
                player = GameObject.FindGameObjectWithTag("Player");
                if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;
                Move();
            }
            else if (Time.time >= lastAttackTime + 1 / attackRate)
            {
                // 플레이어를 발견했고 공격범위 안에 있는 경우, 공격 속도에 맞게 공격 액션을 취한다.
                Attack();
                lastAttackTime = Time.time;
            }
        }
        Flip();
    }
    private void Init()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", isAttacking);

        //canMove = true;
    }
    private void Stop()
    {
        //canMove = false;
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);
    }
    protected void Flip()
    {
        if (movementX >= 0 && transform.rotation.y != 0f) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (movementX < 0 && transform.rotation.y == 0f) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
    protected void Move()
    {
        Vector3 targetVelocity;

        targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        animator.SetFloat("Speed", Mathf.Abs(movementX));
    }
    protected void Attack()
    {
        // 방향 전환
        player = GameObject.FindGameObjectWithTag("Player");
        if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;

        animator.SetTrigger("Attack");
        isAttacking = true;
        animator.SetBool("IsAttacking", isAttacking);

        Stop();
    }
    private void OnSuperArmor()
    {
        superArmor = true;
        //
    }
    private void OffSuperArmor()
    {
        superArmor = false;
        //
    }

    public void TakeDamage(float damage, Vector2 damageForce)
    {
        if (!animator.GetBool("IsDead"))
        {
            if (damage_sound.Length > 0)
            {
                int rand = Random.Range(0, damage_sound.Length);
                if (damage_sound[rand] != null) damage_sound[rand].PlayOneShot(damage_sound[rand].clip);
            }

            if (!superArmor) rb.AddForce(damageForce, ForceMode2D.Impulse);

            if (!isAttacking) animator.SetTrigger("Hit");

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
        Stop();

        // 골드 생성
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Player>().GetGold(100);

        // 체력바 반납
        UI_Container.Instance.AddToEnemySliderPool(healthbar);
    }
    private void destoryObject()
    {
        Destroy(gameObject);
    }
}
