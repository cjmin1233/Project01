using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Default : Enemy_Default
{
    [SerializeField] private Boss_Healthbar boss_healthbar;
    [SerializeField] private GameObject spell_1;
    [SerializeField] private GameObject spell_2;

    private int actionCounter;
    private int random;
    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (maxHP > 0) curHP = maxHP;
        boss_healthbar.SetHealth(curHP, maxHP);

        canMove = true;
        isAttacking = false;
        actionCounter = 0;
        currentSpeed = moveSpeed_multiplier * baseSpeed;
    }
    protected override void Update()
    {
        if (!animator.GetBool("IsDead"))
        {
            canMove = false;
            if (actionCounter < 3)
            {
                for (int i = 0; i < range.Length; i++)
                {
                    if (range[i])
                    {
                        canMove = false;
                        if (Time.time >= lastAttackTime + 1 / attackRate && !isAttacking)
                        {
                            LookPlayer();
                            animator.SetTrigger("Skill" + i);
                            isAttacking = true;
                            animator.SetBool("IsAttacking", isAttacking);
                            lastAttackTime = Time.time;
                        }
                        break;
                    }
                }
            }
            else if (Time.time >= lastAttackTime + 1 / attackRate && !isAttacking)
            {
                canMove = false;
                // 메인 패턴
                random = Random.Range(0, 2);
                animator.SetTrigger("Main_Skill" + random);
                isAttacking = true;
                animator.SetBool("IsAttacking", isAttacking);
                lastAttackTime = Time.time;
            }
            if (!range[0] && !range[1] && !isAttacking) canMove = true;
            currentSpeed = moveSpeed_multiplier * baseSpeed;
            if (canMove)
            {
                movementX = currentSpeed;
                LookPlayer();
            }
            else movementX = 0f;
            animator.SetFloat("Speed", Mathf.Abs(movementX));
            Flip();
        }
    }
    private void Fin_Skill()
    {
        actionCounter++;
    }
    private void Fin_Main_Skill()
    {
        actionCounter = 0;
    }

    private void Spell01()
    {
        spell_1.SetActive(true);
    }
    private void Spell02()
    {
        spell_2.SetActive(true);
    }

    public override void TakeDamage(float damage, Vector2 damageForce)
    {
        if (!animator.GetBool("IsDead"))
        {
            if (damage_sound.Length > 0)
            {
                int rand = Random.Range(0, damage_sound.Length);
                if (damage_sound[rand] != null) damage_sound[rand].PlayOneShot(damage_sound[rand].clip);
            }

            if (damageForce.x != 0f && !isAttacking) animator.SetTrigger("Hit");

            GameObject dmgText = DamageTextPool.Instance.GetFromPool();
            dmgText.transform.position = damagePoint.transform.position;
            dmgText.GetComponent<DamageText>().damage = damage;
            dmgText.SetActive(true);
            curHP -= damage;
            boss_healthbar.SetHealth(curHP, maxHP);
            if (curHP <= 0)
            {
                curHP = 0;
                Die();
            }

        }
    }
    protected override void Die()
    {
        animator.SetBool("IsDead", true);
        Debug.Log("Boss Dead. Congratulations!");
        canMove = false;

        Time.timeScale = 0.5f;
    }
    private void Die_Fin()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
