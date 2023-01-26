using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight_Attack : PlayerAttack
{
    [SerializeField] private GameObject[] comboCollider;
    [SerializeField] private GameObject Sword_Collider_X;
    [SerializeField] private GameObject swordwindPrefab;
    [SerializeField] private Transform sword_wind_startpoint;
    [SerializeField] private AudioSource[] sword_wind_sound;
    [SerializeField] private AudioSource[] Charge_Sound;
    [SerializeField] private GameObject ChargeEffect;
    private bool isCharging = false;
    private int chargeCounter = 0;

    private float combo_coef = 0.4f;
    private float sp_coef = 1.5f;
    private float wind_coef = 0.2f;

    protected override void Update()
    {
        isDashing = animator.GetBool("IsDashing");
        isJumping = animator.GetBool("IsJumping");

        if (sword_storm_enable && Input.GetButton("AttackZ") && !isZAttacking && !isJumping && !isXAttacking && !isDashing && comboCounter == 4)
        {
            SwordZAttack();
        }
        else if (!isJumping && !isXAttacking && !isDashing && comboCounter < 3)
        {
            if (Input.GetButtonDown("AttackZ"))
            {
                if (isZAttacking) inputZCounter++;
                else SwordZAttack();
            }
            //  over Z input handle
            else if (inputZCounter > 0 && !isZAttacking) SwordZAttack();
        }
        /*else if (Input.GetButtonDown("AttackZ") && !isJumping && !isXAttacking && !isDashing && comboCounter < 3)
        {
            if (isZAttacking) inputZCounter++;
            else SwordZAttack();
        }*/
        if (Input.GetButtonDown("AttackX") && !isXAttacking && !isZAttacking && !isJumping && !isDashing)
        {
            // Z+X 콤보
            if (comboCounter == 3)
            {
                gameObject.GetComponent<Player>().canMove = false;
                animator.SetFloat("Speed_X", Speed_X);
                isXAttacking = true;
                animator.SetTrigger("Combo3");

                rb.AddForce(new Vector2(transform.right.x * 1f, 0f), ForceMode2D.Impulse);
            }
            // 일반 X
            else
            {
                //  Sword X attack.
                SwordXAttack();
            }
        }
        if (Input.GetButtonUp("AttackX") && isXAttacking && !isZAttacking && !isJumping && !isDashing && isCharging)
        {
            isCharging = false;
            animator.SetBool("IsCharging", isCharging);
        }
        /*//  over Z input handle
        if (!isZAttacking && !isJumping && !isXAttacking && !isDashing && comboCounter < 3 && inputZCounter > 0)
        {
            SwordZAttack();
            inputZCounter = 0;
        }*/
    }
    private void Finish_X_Charging()
    {
        if (chargeCounter < 3)
        {
            // charge sound
            if (Charge_Sound[chargeCounter] != null) Charge_Sound[chargeCounter].PlayOneShot(Charge_Sound[chargeCounter].clip);
            ChargeEffect.GetComponent<Animator>().SetTrigger("Enable");
            chargeCounter++;
        }
    }
    private void Enable_Sword_Combo_Collider()
    {
        Vector2 damageForce = new Vector2(transform.right.x * 20f, 0f);
        int count = comboCounter % 3;
        comboCollider[count].GetComponent<Combo_Collider>().damage = Mathf.Round(playerPower * damage_z_multiplier * combo_coef * (1 + (float)count * 0.2f));
        comboCollider[count].GetComponent<Combo_Collider>().damageForce = damageForce;
    }
    private void ShootSwordWind()
    {
        if (sword_wind_enable)
        {
            GameObject swordwind = SwordWindPool.Instance.GetFromPool();

            swordwind.GetComponent<Sword_Wind_Collider>().damage = Mathf.Round(playerPower * damage_z_multiplier * wind_coef * (1 + (float)(comboCounter % 3) * 0.2f));
            swordwind.GetComponent<Sword_Wind_Collider>().anim_Speed = Speed_Z;

            swordwind.transform.position = sword_wind_startpoint.position;
            swordwind.SetActive(true);
            int rand = Random.Range(0, sword_wind_sound.Length);
            if (sword_wind_sound[rand] != null) sword_wind_sound[rand].PlayOneShot(sword_wind_sound[rand].clip);
        }
    }
    private void SwordZAttack()
    {
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;

        animator.SetFloat("Speed_Z", Speed_Z);
        isZAttacking = true;
        animator.SetTrigger("Combo" + comboCounter);
        if (comboCounter == 0)
        {
            animator.ResetTrigger("Combo1");
            animator.ResetTrigger("Combo2");
            animator.ResetTrigger("Combo3");
            animator.ResetTrigger("Combo4");
        }
        // 공격시 약 전진

        rb.AddForce(new Vector2(transform.right.x * 10f, 0f), ForceMode2D.Impulse);
    }
    private void Enable_Sword_Collider_X()
    {
        Vector2 damageForce = new Vector2(transform.right.x * 20f, 0f);
        float damage = 0f;
        damage = Mathf.Round(playerPower * damage_x_multiplier * sp_coef * (1 + 0.5f * (float)chargeCounter));
        if (sword_critical_enable)
        {
            int rand = Random.Range(1, 101);
            if (rand <= 40)
            {
                Sword_Collider_X.GetComponent<Combo_Collider>().critical = true;
                damage = Mathf.Round(damage * 1.5f);
            }
            else Sword_Collider_X.GetComponent<Combo_Collider>().critical = false;
        }
        Sword_Collider_X.GetComponent<Combo_Collider>().damage = damage;
        Sword_Collider_X.GetComponent<Combo_Collider>().damageForce = damageForce;
        
    }
    private void SwordXAttack()
    {
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;

        animator.SetFloat("Speed_X", Speed_X);
        isXAttacking = true;
        if (sword_charging_enable)
        {
            animator.SetTrigger("AttackX_Charge_In");
            isCharging = true;
            animator.SetBool("IsCharging", isCharging);
            chargeCounter = 0;
        }
        else animator.SetTrigger("AttackX");
        isZAttacking = false;
        comboCounter = 0;


        rb.AddForce(new Vector2(transform.right.x * 1f, 0f), ForceMode2D.Impulse);
    }

}
