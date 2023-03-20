using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Default : Enemy_Default
{
    [SerializeField] private GameObject boss_healthbar;
    [SerializeField] private GameObject spell_1;
    [SerializeField] private GameObject spell_2;

    private int actionCounter;
    private int random;
    protected override void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        if (maxHP > 0) curHP = maxHP;
        boss_healthbar.GetComponent<Boss_Healthbar>().SetHealth(curHP, maxHP);
        debuff_container = boss_healthbar.transform.Find("Debuff_Container").gameObject;

        canMove = true;
        isAttacking = false;
        actionCounter = 0;
        currentSpeed = moveSpeed_multiplier * baseSpeed;
    }
    protected override void Update()
    {
        if (!animator.GetBool("IsDead"))
        {
            DebuffChecker();
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
                // ���� ����
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

    public override void TakeDamage(float damage, Vector2 damageForce, bool isCrit, Color damageColor, int fxType)
    {
        if (!animator.GetBool("IsDead"))
        {
            if (damage_sound.Length > 0)
            {
                int rand = Random.Range(0, damage_sound.Length);
                if (damage_sound[rand] != null) damage_sound[rand].PlayOneShot(damage_sound[rand].clip);
            }

            if (Mathf.Abs(damageForce.x) > 0.01f && !isAttacking) animator.SetTrigger("Hit");

            #region ������ �ؽ�Ʈ ����
            GameObject dmgText = DamageTextPool.Instance.GetFromPool();
            dmgText.transform.position = damagePoint.transform.position;
            dmgText.GetComponent<DamageText>().damage = damage;
            dmgText.GetComponent<DamageText>().x_dir = damageForce.normalized.x;
            dmgText.GetComponent<DamageText>().textColor = damageColor;
            dmgText.SetActive(true);
            #endregion

            #region Ÿ�� ����Ʈ ����
            if (fxType >= 0)
            {
                GameObject hit_effect = HitFxPool.Instance.GetFromPool();
                float x_rand = 0.5f * Random.Range((-1f) * boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.x);
                float y_rand = 0.5f * Random.Range((-1f) * boxCollider2D.bounds.extents.y, boxCollider2D.bounds.extents.y);
                Vector3 temp = new Vector3(boxCollider2D.bounds.center.x + x_rand, boxCollider2D.bounds.center.y + y_rand, 0);
                hit_effect.transform.position = temp;
                hit_effect.GetComponent<HitFx>().fxType = fxType;
                hit_effect.SetActive(true);
            }
            #endregion

            curHP -= damage;
            boss_healthbar.GetComponent<Boss_Healthbar>().SetHealth(curHP, maxHP);
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
        GameManager.Instance.bossFollowing = true;
        Debug.Log("Boss Dead. Congratulations!");
        canMove = false;
        baseSpeed = 0f;
    }
    private void Die_Fin()
    {
        GameManager.Instance.bossFollowing = false;
        boss_healthbar.SetActive(false);

        Destroy(gameObject);
    }
}
