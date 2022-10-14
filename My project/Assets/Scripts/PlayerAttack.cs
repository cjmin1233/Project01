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


    //private int SwordDamage = 40;
    public int swordDamage_z = 40;
    public int swordDamage_x = 40;

    // Combo attack ***************
    public int comboCounter = 0;
    public bool isZAttacking = false;
    public float Speed_Z = 1.0f;
    // ****************************
    
    // X attack *******************
    public bool isXAttacking = false;
    public float Speed_X = 1.0f;
    // ****************************
    int weaponType;
    bool isJumping;
    bool isDashing;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }
    void Update()
    {
        isDashing = animator.GetBool("IsDashing");
        isJumping = animator.GetBool("IsJumping");
        weaponType = animator.GetInteger("WeaponType");

        if (Input.GetButtonDown("AttackZ") && !isZAttacking && !isJumping && !isXAttacking && !isDashing)
        {
            if (weaponType == 1)
            {
                //  Sword Z combo attack.
                SwordZAttack();
            }
            else if (weaponType == 2)
            {
                //  Bow Z combo attack.
            }
            else if (weaponType == 3)
            {
                //  Spear Z combo attack.
            }
        }

        if (Input.GetButton("AttackX") && !isXAttacking && !isJumping && !isZAttacking && !isDashing)
        {
            if (weaponType == 1)
            {
                //  Sword X attack.
                SwordXAttack();
            }
            else if (weaponType == 2)
            {
                //  Bow X attack.
                BowXAttack();
            }
            else if (weaponType == 3)
            {
                //  Spear X attack.
            }

        }

    }
    public void Start_Combo()
    {
        isZAttacking = false;
        if (comboCounter < 3)
        {
            comboCounter++;
        }
    }
    public void Finish_Combo()
    {
        isZAttacking = false;
        comboCounter = 0;
    }
    public void Finish_X()
    {
        isXAttacking = false;
    }
    private void SwordZAttack()
    {
        animator.SetFloat("Speed_Z", Speed_Z);
        isZAttacking = true;
        animator.SetTrigger("Combo" + comboCounter);

        //Debug.Log(comboCounter);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(SwordPoint.position, SwordRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            //Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(swordDamage_z);
        }
    }
    private void SwordXAttack()
     {
        animator.SetFloat("Speed_X", Speed_X);
        isXAttacking = true;
        animator.SetTrigger("AttackX");
        Finish_Combo();

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(SwordPoint.position, SwordRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            //Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(swordDamage_x);
        }
    }
    private void BowXAttack()
    {
        animator.SetFloat("Speed_X", Speed_X);
        isXAttacking = true;
        animator.SetTrigger("AttackX");
        Finish_Combo();
        Instantiate(ArrowPrefab, firePoint.position, firePoint.rotation);
    }



    /*
    public void BigArrow()
    {
        Debug.Log("Make the arrow Bigger");
        ArrowPrefab.GetComponent<BoxCollider2D>().size = new Vector2(ArrowPrefab.GetComponent<BoxCollider2D>().size.x, ArrowPrefab.GetComponent<BoxCollider2D>().size.y * 1.5f);
    }*/
    /*
    private void OnDrawGizmos()
    {
        if (SwordPoint == null) return;
        Gizmos.DrawWireSphere(SwordPoint.position, SwordRange);

        //Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
    */
}
