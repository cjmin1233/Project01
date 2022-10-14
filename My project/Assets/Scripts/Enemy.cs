using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    [SerializeField] private Transform damagePoint;
    public GameObject DamageText;
    public Enemy_Healthbar healthbar;

    private void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetHealth(maxHealth, maxHealth);
        animator = GetComponent<Animator>();
        //rg = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
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
    void Die()
    {
        animator.SetBool("IsDead", true);
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
