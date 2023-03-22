using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Default : MonoBehaviour
{
    [Header("EnemyType")] public int enemyType;
    protected GameObject player;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected BoxCollider2D boxCollider2D;

    // 이동 관련
    protected float currentSpeed;
    [SerializeField] protected float baseSpeed = 100f;
    [HideInInspector] public float moveSpeed_multiplier = 1f;
    public Dictionary<string, float> debuffer = new Dictionary<string, float>();
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
    protected GameObject debuff_container;
    [SerializeField] protected Vector3 Offset;

    // 소리
    [Header("Audio Source")] [SerializeField] private AudioSource die_sound;
    [SerializeField] protected AudioSource[] damage_sound;

    // 독뎀
    private float lastPoisonDamageTime = 0f;

    // 추락 방지
    private RaycastHit2D GroundRayHit;
    private Vector3 safePos;
    protected virtual void OnEnable()
    {
        #region 초기 세팅
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        if (maxHP > 0) curHP = maxHP;
        healthbar = UI_Container.Instance.GetFromEnemySliderPool();
        healthbar.GetComponent<Enemy_Healthbar>().SetHealth(curHP, maxHP);
        debuff_container = healthbar.transform.Find("Debuff_Container").gameObject;

        canMove = true;
        isAttacking = false;
        for (int i = 0; i < range.Length; i++) range[i] = false;
        currentSpeed = moveSpeed_multiplier * baseSpeed;
        #endregion
    }
    protected virtual void Update()
    {
        // 체력바 이동
        healthbar.GetComponent<Slider>().transform.position = Camera.main.WorldToScreenPoint(transform.position + Offset);
        if (!animator.GetBool("IsDead"))
        {
            DebuffChecker();
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
        }
    }
    protected virtual void FixedUpdate()
    {
        GroundRayHit = Physics2D.Raycast(new Vector2(boxCollider2D.bounds.center.x, boxCollider2D.bounds.min.y), Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        if (GroundRayHit.collider != null)
        {
            //Debug.Log("Touching ground");
            safePos = transform.position;
            rb.gravityScale = 1f;
        }
        else if (safePos != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, safePos, 0.9f);
            rb.gravityScale = 0f;
        }
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
        if (transform.position.x < player.transform.position.x) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        //if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;
    }
    protected void Flip()
    {
        if (movementX > 0 && transform.rotation.y != 0f) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (movementX < 0 && transform.rotation.y == 0f) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
    protected virtual void Move()
    {
        Vector3 targetVelocity;

        targetVelocity = new Vector2(movementX * Time.fixedDeltaTime * transform.right.x, rb.velocity.y);
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

    public virtual void TakeDamage(float damage, Vector2 damageForce, bool isCrit, Color damageColor, int fxType)
    {
        if (!animator.GetBool("IsDead"))
        {
            if (damage_sound.Length > 0)
            {
                int rand = Random.Range(0, damage_sound.Length);
                if (damage_sound[rand] != null) damage_sound[rand].PlayOneShot(damage_sound[rand].clip);
            }

            // 슈퍼아머가 아니라면 밀려남
            if (!superArmor) rb.AddForce(damageForce, ForceMode2D.Impulse);
            // 밀치는 힘이 있다면 타격 애니메이션
            if (Mathf.Abs(damageForce.x) > 0.01f) animator.SetTrigger("Hit");
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
        rb.velocity = Vector2.zero;
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
    public void Debuff(string name, float activeTime)
    {
        if (debuffer.ContainsKey(name)) debuffer[name] = activeTime;
        else debuffer.Add(name, activeTime);
    }
    protected void DebuffChecker()
    {
        if (debuffer.ContainsKey("Poison"))
        {
            if (debuffer["Poison"] > 0f)
            {
                debuffer["Poison"] -= Time.deltaTime;
                Poisoned();
            }
            else debuffer["Poison"] = 0f;
        }
        // 다른 디버프
        if (debuffer.ContainsKey("Slow"))
        {
            if (debuffer["Slow"] > 0f)
            {
                debuffer["Slow"] -= Time.deltaTime;
                moveSpeed_multiplier = 0.5f;
            }
            else
            {
                debuffer["Slow"] = 0f;
                moveSpeed_multiplier = 1f;
            }
        }
        //
        if (debuff_container != null) debuff_container.GetComponent<Debuff_Container>().UpdateDebuffIcon(debuffer);
    }
    private void Poisoned()
    {
        if (Time.time - lastPoisonDamageTime >= 1f)
        {
            lastPoisonDamageTime = Time.time;
            TakeDamage(10f, Vector2.zero, false, Color.magenta, -1);
            Debug.Log("<color=purple>독 데미지</color>");
        }
    }
    private void destoryObject()
    {
        //Destroy(gameObject);
        EnemyPool.Instance.AddToPool(enemyType, gameObject);
        EnemyPool.Instance.remainEnemies--;
    }
}