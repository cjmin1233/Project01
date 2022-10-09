using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;
    public Transform SwordPoint;
    public float SwordRange = 0.5f;
    public LayerMask enemyLayers;

    [SerializeField] private Transform firePoint;
    public GameObject ArrowPrefab;


    private int SwordDamage = 40;
    float nextAttackTime = 0f;

    private float XattackTime = 0.5f;
    private float swordCombo_1 = 0.4f;
    private float swordCombo_2 = 0.3f;
    private float swordCombo_3 = 0.5f;

    private float comboCounter = 0f;
    private float comboTime = 0.5f;
    // float comboTimeLeft;
    int weaponType;
    bool isZAttacking = false;
    bool isXAttacking = false;


    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            isZAttacking = false;
            isXAttacking = false;
        }


        if (Input.GetButtonDown("AttackZ") && Time.time >= nextAttackTime)
        {
            weaponType = animator.GetInteger("WeaponType");
            if (weaponType == 1 && !isXAttacking)
            {
                isZAttacking = true;
                //Debug.Log("Combo Attack");
                if (comboCounter <= 0.5f || Time.time > nextAttackTime + comboTime)
                {
                    // Combo_1
                    comboCounter = 0f;
                    nextAttackTime = Time.time + swordCombo_1;
                    //comboTimeLeft = comboTime;
                    animator.SetFloat("Combo", comboCounter);
                    comboCounter = 1.0f;
                }
                else if (comboCounter <= 1.5f && Time.time <= nextAttackTime + comboTime)
                {
                    // Combo_2
                    nextAttackTime = Time.time + swordCombo_2;
                    //comboTimeLeft = comboTime;
                    animator.SetFloat("Combo", comboCounter);
                    comboCounter += 1.0f;
                }
                else if(comboCounter<=2.5f && Time.time <= nextAttackTime + comboTime)
                {
                    // Combo_3
                    nextAttackTime = Time.time + swordCombo_3;
                    animator.SetFloat("Combo", comboCounter);
                    comboCounter = 0f;
                }
                SwordZAttack();
            }
        }

        if (Time.time >= nextAttackTime && !isZAttacking)
        {
            weaponType = animator.GetInteger("WeaponType");
            if (weaponType == 1)
            {
                if (Input.GetButtonDown("AttackX"))
                {
                    isXAttacking = true;
                    //Debug.Log("Sword attack");
                    SwordXAttack();
                    nextAttackTime = Time.time + XattackTime;
                }

            }
            else if (weaponType == 2)
            {
                if (Input.GetButtonDown("AttackX"))
                {
                    //Debug.Log("Bow attack");
                    BowXAttack();
                    nextAttackTime = Time.time + XattackTime;
                }

            }
        }

        //comboTimeLeft -= Time.fixedDeltaTime;

    }
    private void SwordZAttack()
    {
        animator.SetTrigger("AttackZ");
        //Debug.Log(comboCounter);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(SwordPoint.position, SwordRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            //Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(SwordDamage);
        }
    }
    private void SwordXAttack()
     {
        animator.SetTrigger("AttackX");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(SwordPoint.position, SwordRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            //Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(SwordDamage);
        }
    }
    private void BowXAttack()
    {
        animator.SetTrigger("AttackX");
        Instantiate(ArrowPrefab, firePoint.position, firePoint.rotation);
    }
    public void BigArrow()
    {
        Debug.Log("Make the arrow Bigger");
        ArrowPrefab.GetComponent<BoxCollider2D>().size = new Vector2(ArrowPrefab.GetComponent<BoxCollider2D>().size.x, ArrowPrefab.GetComponent<BoxCollider2D>().size.y * 1.5f);
    }
    /*
    private void OnDrawGizmos()
    {
        if (SwordPoint == null) return;
        Gizmos.DrawWireSphere(SwordPoint.position, SwordRange);

        //Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
    */
}
