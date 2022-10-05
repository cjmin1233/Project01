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
    private float attackRate = 2f;
    float nextAttackTime = 0f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            int weaponType = animator.GetInteger("WeaponType");
            if (weaponType == 1)
            {
                if (Input.GetButtonDown("Attack"))
                {
                    //Debug.Log("Sword attack");
                    SwordAttack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }

            }
            else if (weaponType == 2)
            {
                if (Input.GetButtonDown("Attack"))
                {
                    //Debug.Log("Bow attack");
                    BowAttack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }

            }
        }


        void SwordAttack()
        {
            animator.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(SwordPoint.position, SwordRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                //Debug.Log("We hit " + enemy.name);
                enemy.GetComponent<Enemy>().TakeDamage(SwordDamage);
            }
        }
        void BowAttack()
        {
            animator.SetTrigger("Attack");
            Instantiate(ArrowPrefab, firePoint.position, firePoint.rotation);
        }
        /*
        void OnDrawGizmosSelected()
        {
            if (SwordPoint == null) return;
            Gizmos.DrawWireSphere(SwordPoint.position, SwordRange);
        }*/

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
