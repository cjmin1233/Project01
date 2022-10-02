using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private Animator animator;
    public Transform SwordPoint;
    public float SwordRange = 0.5f;
    public LayerMask enemyLayers;

    private int SwordDamage = 40;
    private float attackRate = 2f;
    float nextAttackTime = 0f;

    private void Awake()
    {
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
                    Debug.Log("attack");
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }

            }
        }

        
        void Attack()
        {
            animator.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(SwordPoint.position, SwordRange, enemyLayers);

            foreach(Collider2D enemy in hitEnemies)
            {
                //Debug.Log("We hit " + enemy.name);
                enemy.GetComponent<Enemy>().TakeDamage(SwordDamage);
            }
        }
        /*
        void OnDrawGizmosSelected()
        {
            if (SwordPoint == null) return;
            Gizmos.DrawWireSphere(SwordPoint.position, SwordRange);
        }*/

    }
}
