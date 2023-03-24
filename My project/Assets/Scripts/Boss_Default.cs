using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Default : Enemy_Default
{
    [SerializeField] private GameObject boss_healthbar;

    private int actionCounter;
    private int random;

    // 보스 스펠 컨테이너
    [SerializeField] private GameObject[] fixed_spell_pos_container;
    [SerializeField] private GameObject[] random_spell_pos_container;
    [SerializeField] private int[] spellTypes;

    // 쫄몹 소환
    [SerializeField] private float enemy_spawn_range;   // 최소 2 이상
    private RaycastHit2D rayHit;
    private float min_x, max_x;

    protected override void OnEnable()
    {
        #region 초기 세팅
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        curHP = maxHP;
        boss_healthbar.GetComponent<Boss_Healthbar>().SetHealth(curHP, maxHP);
        debuff_container = boss_healthbar.transform.Find("Debuff_Container").gameObject;

        canMove = true;
        isAttacking = false;
        actionCounter = 0;
        currentSpeed = moveSpeed_multiplier * baseSpeed;
        #endregion
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
                            // 일반 패턴. 거리에 따른 우선도
                            animator.SetTrigger("Skill" + i);
                            isAttacking = true;
                            animator.SetBool("IsAttacking", isAttacking);
                            //
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
                random = Random.Range(0, 3);
                animator.SetTrigger("Main_Skill" + random);
                isAttacking = true;
                animator.SetBool("IsAttacking", isAttacking);
                //

                lastAttackTime = Time.time;
            }
            ////////////////////////////////////////수정 필요?
            if (!range[0] && !range[1] && !isAttacking) canMove = true;

            currentSpeed = moveSpeed_multiplier * baseSpeed;
            if (canMove)
            {
                movementX = currentSpeed;
                LookPlayer();
            }
            else movementX = 0f;
            animator.SetFloat("Speed", Mathf.Abs(movementX));
        }
    }
    protected override void FixedUpdate()
    {
        rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), direction: Vector2.right, distance: enemy_spawn_range, layerMask: LayerMask.GetMask("StaticMap"));
        // 오른쪽 벽이 범위 안쪽인 경우
        if (rayHit.collider != null) max_x = rayHit.distance - 2f;
        // 오른쪽 벽이 범위 밖인 경우
        else max_x = enemy_spawn_range;
        rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), direction: Vector2.left, distance: enemy_spawn_range, layerMask: LayerMask.GetMask("StaticMap"));
        // 왼쪽 벽이 범위 안쪽인 경우
        if (rayHit.collider != null) min_x = -rayHit.distance + 2f;
        // 왼쪽 벽이 범위 밖인 경우
        else min_x = -enemy_spawn_range;

        Move();
    }
    private void Fin_Skill()
    {
        actionCounter++;
    }
    private void Fin_Main_Skill()
    {
        actionCounter = 0;
    }
    private void RandomSpell()
    {
        int rand = Random.Range(0, spellTypes.Length);
        Vector3 temp = random_spell_pos_container[rand].transform.position;
        temp.x = player.transform.position.x;
        SpellSpawn(spellTypes[rand], temp);
    }
    private void SpellSpawn(int type, Vector3 spawnPos)
    {
        GameObject gameObject = EnemyBulletPool.Instance.GetFromPool();
        gameObject.GetComponent<Enemy_Bullet>().damage = 10f;
        gameObject.GetComponent<Enemy_Bullet>().type = type;
        gameObject.GetComponent<Enemy_Bullet>().bulletSpeed = new Vector2(0f, 0f);
        gameObject.transform.position = spawnPos;
        gameObject.SetActive(true);
    }
    private void FixedSpell(int index)
    {
        GameObject parentGameObject = fixed_spell_pos_container[index];
        int childCount = parentGameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform transform = parentGameObject.transform.GetChild(i);
            SpellSpawn(2, transform.position);
        }
    }
    private void EnemySummon(int num)
    {
        Debug.Log(min_x + "~" + max_x);
        for(int i = 0; i < num; i++)
        {
            var enemy = EnemyPool.Instance.GetFromPool(2);
            float range = Random.Range(0, max_x - min_x);
            Debug.Log("소환 위치: " + range + " / " + (max_x - min_x));
            enemy.transform.position = new Vector3(transform.position.x + min_x + range, transform.position.y, 0f);
            enemy.SetActive(true);
        }
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

            #region 데미지 텍스트 생성
            GameObject dmgText = DamageTextPool.Instance.GetFromPool();
            dmgText.transform.position = damagePoint.transform.position;
            dmgText.GetComponent<DamageText>().damage = damage;
            dmgText.GetComponent<DamageText>().x_dir = damageForce.normalized.x;
            dmgText.GetComponent<DamageText>().textColor = damageColor;
            dmgText.SetActive(true);
            #endregion

            #region 타격 이펙트 생성
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
                Die();
            }

        }
    }
    protected override void Die()
    {
        curHP = 0;
        if (die_sound != null) die_sound.PlayOneShot(die_sound.clip);
        animator.SetBool("IsDead", true);
        GameManager.Instance.bossFollowing = true;
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
