using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Rigidbody2D rg;
    private Animator animator;
    private SpriteRenderer sr;
    //public Transform SwordPoint;
    public float SwordRange = 0.5f;
    public LayerMask enemyLayers;

    [SerializeField] private Transform firePoint;
    public GameObject ArrowPrefab;


    //private int SwordDamage = 40;
    public float swordDamage_z = 40;
    public float swordDamage_x = 40;
    public float swordDamage_z_multiplier = 1.0f;
    public float swordDamage_x_multiplier = 1.0f;


    // Combo attack ***************
    public int comboCounter = 0;
    public bool isZAttacking = false;
    public float Speed_Z = 1.0f;
    public GameObject[] comboCollider;
    // ****************************
    
    // X attack *******************
    public bool isXAttacking = false;
    public float Speed_X = 1.0f;
    public GameObject Sword_Collider_X;
    // ****************************
    int weaponType;
    bool isJumping;
    bool isDashing;

    //public float swordCombo_force = 100f;

    private void Start()
    {
        rg = GetComponent<Rigidbody2D>();
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

        if (Input.GetButtonDown("AttackX") && !isXAttacking && !isJumping && !isZAttacking && !isDashing)
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
        Enable_Sword_Combo_Collider();
        if (comboCounter < 3)
        {
            comboCounter++;
        }
    }
    public void Finish_Combo()
    {
        isZAttacking = false;
        comboCounter = 0;
        gameObject.GetComponent<Player>().canMove = true;
    }
    public void Finish_X()
    {
        isXAttacking = false;
        gameObject.GetComponent<Player>().canMove = true;
    }

    private void Enable_Sword_Combo_Collider()
    {
        comboCollider[comboCounter].GetComponent<Sword_Combo_Collider>().damage = Mathf.Round(swordDamage_z * swordDamage_z_multiplier);
        comboCollider[comboCounter].GetComponent<Sword_Combo_Collider>().anim_Speed = Speed_Z;
        comboCollider[comboCounter].SetActive(true);
    }
    private void SwordZAttack()
    {
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;

        animator.SetFloat("Speed_Z", Speed_Z);
        isZAttacking = true;
        animator.SetTrigger("Combo" + comboCounter);

        // 공격시 약 전진
        //Debug.Log("i'm here");
        float swordCombo_force = 20f;
        if (transform.rotation.y != 0f) swordCombo_force *= -1f;
        rg.AddForce(new Vector2(swordCombo_force, 0f), ForceMode2D.Impulse);
        //rg.AddForce(new Vector2(swordCombo_force, 0f), ForceMode2D.Force);

        //Debug.Log(comboCounter);

        /*
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(SwordPoint.position, SwordRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            string tag = enemy.tag;
            if (tag == "Enemy")
            {
                enemy.GetComponent<Enemy>().TakeDamage(Mathf.Round(swordDamage_z * swordDamage_z_multiplier));
                //enemy.GetComponent<Enemy>().TakeDamage(swordDamage_z);
            }
            else if (tag == "Boss")
            {
                //Debug.Log("We hit " + enemy.name);
                enemy.GetComponent<Boss>().TakeDamage(Mathf.Round(swordDamage_z * swordDamage_z_multiplier));
            }
            else
            {
                Debug.Log("We hit nothing");
            }
            //Debug.Log("We hit " + enemy.name);
        }
         */
    }
    private void Enable_Sword_Collider_X()
    {
        Sword_Collider_X.GetComponent<Sword_Combo_Collider>().damage = Mathf.Round(swordDamage_x * swordDamage_x_multiplier);
        Sword_Collider_X.GetComponent<Sword_Combo_Collider>().anim_Speed = Speed_X;
        Sword_Collider_X.SetActive(true);
    }
    private void SwordXAttack()
     {
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;

        animator.SetFloat("Speed_X", Speed_X);
        isXAttacking = true;
        animator.SetTrigger("AttackX");
        Finish_Combo();

        float swordCombo_force = 0.1f;
        if (transform.rotation.y != 0f) swordCombo_force *= -1f;
        rg.AddForce(new Vector2(swordCombo_force, 0f), ForceMode2D.Impulse);

        /*
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(SwordPoint.position, SwordRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            string tag = enemy.tag;
            if (tag == "Enemy")
            {
                enemy.GetComponent<Enemy>().TakeDamage(Mathf.Round(swordDamage_x * swordDamage_x_multiplier));
            }
            else if (tag == "Boss")
            {
                Debug.Log("We hit " + enemy.name);
                enemy.GetComponent<Boss>().TakeDamage(Mathf.Round(swordDamage_x * swordDamage_x_multiplier));
            }
            else
            {
                Debug.Log("We hit nothing");
            }
        }
         */
    }
    private void BowXAttack()
    {
        // 공격동안 움직임 제어
        //gameObject.GetComponent<Player>().canMove = false;

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
