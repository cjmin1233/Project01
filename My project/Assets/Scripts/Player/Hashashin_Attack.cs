using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hashashin_Attack : PlayerAttack
{
    [SerializeField] private GameObject[] comboCollider;
    [SerializeField] private GameObject[] Sword_Collider_X;

    private float combo_coef = 0.2f;
    private float sp_coef = 1f;

    private float curGauge = 0f;
    private float maxGauge = 100f;
    private float reqGauge = 33f;


    protected override void Start()
    {
        base.Start();
        UI_Container.Instance.updateGauge(curGauge);
    }
    protected override void Update()
    {
        isDashing = animator.GetBool("IsDashing");
        isJumping = animator.GetBool("IsJumping");

        /*if (Input.GetKeyDown(KeyCode.S))
        {
            GetGauge(10f);
        }*/
        if (dagger_storm_enable && Input.GetButton("AttackZ") && !isZAttacking && !isJumping && !isXAttacking && !isDashing && comboCounter == 4)
        {
            DaggerZAttack();
        }
        else if (!isJumping && !isXAttacking && !isDashing && comboCounter < 3)
        {
            if (Input.GetButtonDown("AttackZ"))
            {
                if (isZAttacking) inputZCounter++;
                else DaggerZAttack();
            }
            //  over Z input handle
            else if (inputZCounter > 0 && !isZAttacking)
            {
                DaggerZAttack();
                inputZCounter = 0;
            }
        }

        if (Input.GetButtonDown("AttackX") && !isXAttacking && !isZAttacking && !isJumping && !isDashing)
        {
            DaggerXAttack();
        }
    }
    private void DaggerZAttack()
    {
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;
        
        animator.SetFloat("Speed_Z", Z_SpeedCalculation());
        isZAttacking = true;
        animator.SetBool("IsZAttacking", isZAttacking);
        if (comboCounter == 0 && quick_wind_enable) comboCounter = 2;
        animator.SetTrigger("Combo" + comboCounter);
        if (comboCounter == 0)
        {
            animator.ResetTrigger("Combo1");
            animator.ResetTrigger("Combo2");
            animator.ResetTrigger("Combo4");
        }
    }
    private void Enable_Dagger_Combo_Collider()
    {
        Vector2 damageForce = new Vector2(0.1f * transform.right.x, 0f);
        int counter = comboCounter % 3;
        comboCollider[counter].GetComponent<Combo_Collider>().damage = Mathf.Round(PlayerPowerCalculation() * Z_DamageCalculation() * combo_coef * (1 + (float)counter * 0.2f));
        comboCollider[counter].GetComponent<Combo_Collider>().damageForce = damageForce;
    }
    private void DaggerXAttack()
    {
        if (curGauge >= reqGauge)
        {
            // 게이지 소비 후 ui 반영
            curGauge -= reqGauge;
            UI_Container.Instance.updateGauge(curGauge);

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().playerFollowing = false;
            // 공격동안 움직임 제어
            gameObject.GetComponent<Player>().canMove = false;

            animator.SetFloat("Speed_X", X_SpeedCalculation());

            isXAttacking = true;
            animator.SetBool("IsXAttacking", isXAttacking);
            animator.SetTrigger("AttackX");

            // after image on
            gameObject.GetComponent<Player>().AfterImageAvailable = true;
            comboCounter = 0;
        }
        else
        {
            Debug.Log("게이지가 부족합니다.");
        }
    }
    private void Enable_Dagger_X_Collider()
    {
        Vector2 damageForce = new Vector2(1f * transform.right.x, 0f);

        for (int i = 0; i < Sword_Collider_X.Length; i++)
        {
            Sword_Collider_X[i].GetComponent<Combo_Collider>().damage = Mathf.Round(PlayerPowerCalculation() * X_DamageCalculation() * sp_coef);
            Sword_Collider_X[i].GetComponent<Combo_Collider>().damageForce = damageForce;
        }
    }
    public void GetGauge(float gauge)
    {
        curGauge += gauge;
        if (curGauge > maxGauge) curGauge = maxGauge;
        UI_Container.Instance.updateGauge(curGauge);
    }
}
