using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    GameObject player;
    [SerializeField] private GameObject spell_1;
    [SerializeField] private GameObject spell_2;
    SpriteRenderer sr;
    Rigidbody2D rb;
    Animator animator;

    private float movementX = 200f;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_MovementSmoothing = 0.05f;

    /*private bool isActing;
    private float waitTime = 2f;
    private float timer = 0f;
    private int nextAction;*/
    //
    [HideInInspector] public bool playerInRange;
    private bool canMove;
    private bool isAttacking;
    private int actionCounter;

    // Boss HP
    private float maxHealth;
    private float currentHealth;
    [SerializeField] private Transform damagePoint;
    public GameObject DamageText;
    public Boss_Healthbar healthbar;

    //
    int random;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        //isActing = false;

        maxHealth = 2000f;
        currentHealth = maxHealth;
        healthbar.SetHealth(currentHealth, maxHealth);
        //
        canMove = true;
        isAttacking = false;
        actionCounter = 0;
        playerInRange = false;
    }

    private void Update()
    {
        /*if (!isActing) timer += Time.deltaTime;
        if (timer > waitTime)
        {
            int random = Random.Range(0, 2);
            nextAction = random;
            if (player.transform.position.x < gameObject.transform.position.x) movementX = -200.0f;
            else movementX = 200f;

            isActing = true;
            timer = 0f;
        }*/
        if (!isAttacking)
        {
            if (!playerInRange) canMove = true;
            else
            {
                BossStop();
                if (actionCounter < 3)
                {
                    // 짤패턴
                    random = Random.Range(0, 2);
                    animator.SetTrigger("Skill" + random);
                    isAttacking = true;
                }
                else
                {
                    // 메인 패턴
                    random = Random.Range(0, 2);
                    animator.SetTrigger("Main_Skill" + random);
                    isAttacking = true;
                }
            }
        }

        Flip();
    }

    private void FixedUpdate()
    {
        /*if (isActing)
        {
            if (nextAction == 0)
            {
                BossMove();
            }

            else if (nextAction == 1)
            {
                animator.SetBool("Skill_FireWall", true);
            }
        }*/
        if (!animator.GetBool("IsDead") && canMove)
        {
            // 움직임 가능한 경우
            player = GameObject.FindGameObjectWithTag("Player");
            if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;
            BossMove();
        }
    }
    private void BossMove()
    {
        Vector3 targetVelocity;

        targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        animator.SetFloat("Speed", Mathf.Abs(movementX));
    }
    private void BossStop()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);
    }
    /*private void Fin_Act()
    {
        isActing = false;
        animator.SetBool("Skill_FireWall", false);
        rb.velocity = Vector3.zero;
        animator.SetFloat("Speed", 0);
    }*/
    private void Fin_Skill()
    {
        actionCounter++;
    }
    private void Fin_Main_Skill()
    {
        actionCounter = 0;
    }
    private void Boss_Idle_Init()
    {
        canMove = true;
        isAttacking = false;
    }

    private void Spell01()
    {
        spell_1.SetActive(true);
    }
    private void Spell02()
    {
        spell_2.SetActive(true);
    }
    private void Flip()
    {
        if (movementX >= 0 && transform.rotation.y != 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (movementX < 0 && transform.rotation.y == 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void TakeDamage(float damage)
    {
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
        Debug.Log("Boss Dead. Congratulations!");
        animator.SetBool("IsDead", true);
        BossStop();
        Time.timeScale = 0.5f;
    }
    private void Die_Fin()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
