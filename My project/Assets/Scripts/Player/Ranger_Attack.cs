using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_Attack : PlayerAttack
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject ArrowPrefab;
    [SerializeField] private AudioSource[] bow_shoot_sound;
    [SerializeField] private GameObject Bow_Beam;
    [SerializeField] private GameObject arrow_shower_startpoint;
    private RaycastHit2D rayHit;
    private float arrow_coef = 0.5f;
    private float shower_coef = 0.15f;
    private float beam_coef = 0.2f;
    /*protected override void Start()
    {
        base.Start();
        bow_storm_enable = false;
        bow_poison_enable = false;
        bow_air_enable = false;
    }*/
    protected override void Update()
    {
        isDashing = animator.GetBool("IsDashing");
        isJumping = animator.GetBool("IsJumping");

        if (bow_storm_enable && Input.GetButton("AttackZ") && !isZAttacking && !isXAttacking && !isDashing && comboCounter == 4)
        {
            if (isJumping && bow_air_enable)
            {
                BowZAttack_Air();
            }
            else
            {
                BowZAttack();
            }
        }
        else if (Input.GetButtonDown("AttackZ") && !isXAttacking && !isDashing && comboCounter < 3)
        {
            if (isZAttacking)
            {
                // 스택 증가
                inputZCounter++;
            }
            else if (isJumping && bow_air_enable) BowZAttack_Air();
            else if (!isJumping) BowZAttack();
        }

        if (Input.GetButtonDown("AttackX") && !isXAttacking && !isZAttacking && !isJumping && !isDashing)
        {
            // Z+X 콤보
            if (comboCounter == 3)
            {
                gameObject.GetComponent<Player>().canMove = false;
                arrow_shower_startpoint.GetComponent<Arrow_Shower_Startpoint>().anim_Speed = Speed_X;
                animator.SetFloat("Speed_X", Speed_X);
                isXAttacking = true;
                animator.SetTrigger("Combo3");
            }
            // 일반 X
            else BowXAttack();
        }
        /*if (Input.GetButtonUp("AttackX") && isXAttacking && !isZAttacking && !isJumping && !isDashing && isCharging)
        {
            isCharging = false;
            animator.SetBool("IsCharging", isCharging);
        }*/

        //  over Z input handle
        if (!isZAttacking && !isXAttacking && !isDashing && comboCounter < 3 && inputZCounter > 0)
        {
            if (isJumping && bow_air_enable) BowZAttack_Air();
            else if (!isJumping) BowZAttack();
            inputZCounter = 0;
        }
    }
    private void FixedUpdate()
    {
        rayHit = Physics2D.Raycast(rb.position, Vector3.down, 10f, LayerMask.GetMask("Ground"));
        //Debug.Log(rayHit.distance);
    }
    protected override void Finish_Air_Combo()
    {
        isZAttacking = true;
    }
    protected override void BowZAttack()
    {
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;

        animator.SetFloat("Speed_Z", Speed_Z);
        isZAttacking = true;
        if (comboCounter == 0)
        {
            animator.ResetTrigger("Combo1");
            animator.ResetTrigger("Combo2");
            animator.ResetTrigger("Combo3");
            animator.ResetTrigger("Combo4");
        }
        animator.SetTrigger("Combo" + comboCounter);
    }
    protected override void BowZAttack_Air()
    {
        if (rayHit.distance > 3f)
        {
            // 공격동안 움직임 제어
            gameObject.GetComponent<Player>().canMove = false;

            animator.SetFloat("Speed_Z", Speed_Z);
            isZAttacking = true;
            if (comboCounter == 0)
            {
                animator.ResetTrigger("Combo1");
                animator.ResetTrigger("Combo2");
                animator.ResetTrigger("Combo3");
                animator.ResetTrigger("Combo4");
            }
            animator.SetTrigger("Combo" + comboCounter);
            animator.SetBool("IsJumpingDown", true);
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(new Vector2(transform.right.x * (-5f), 3f), ForceMode2D.Impulse);
        }
    }
    protected override void ShootArrow()
    {
        GameObject arrow = ArrowPool.Instance.GetFromPool();
        arrow.GetComponent<Bullet>().damage = Mathf.Round(playerPower * damage_z_multiplier * arrow_coef * (1 + comboCounter * 0.2f));
        arrow.GetComponent<Bullet>().anim_Speed = Speed_Z;
        arrow.GetComponent<Bullet>().isDiagonal = isJumping;
        if (bow_poison_enable) arrow.GetComponent<Bullet>().isPoisoned = true;
        arrow.transform.position = firePoint.position;
        arrow.SetActive(true);
        /*int rand = Random.Range(0, sword_wind_sound.Count);*/
        if (bow_shoot_sound[0] != null) bow_shoot_sound[0].PlayOneShot(bow_shoot_sound[0].clip);
    }
    protected override void BowXAttack()
    {
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;

        animator.SetFloat("Speed_X", Speed_X);
        isXAttacking = true;
        animator.SetTrigger("AttackX");
        isZAttacking = false;
        comboCounter = 0;
    }
    protected override void Enable_Bow_Beam()
    {
        Vector2 damageForce = new Vector2(transform.right.x * 10f, 0f);
        //if (transform.rotation.y != 0f) damageForce.x *= -1f;
        Bow_Beam.GetComponent<Bow_Beam_Collider>().damage = Mathf.Round(playerPower * damage_x_multiplier * beam_coef);
        Bow_Beam.GetComponent<Bow_Beam_Collider>().anim_Speed = Speed_X;
        Bow_Beam.GetComponent<Bow_Beam_Collider>().damageForce = damageForce;
    }
    protected override void Enable_Arrow_Shower()
    {
        Vector2 damageForce = new Vector2(transform.right.x * 8f, 0f);
        GameObject arrowshower = ArrowShowerPool.Instance.GetFromPool();
        arrowshower.GetComponent<Arrow_Shower_Collider>().damage = Mathf.Round(playerPower * damage_x_multiplier * shower_coef);
        arrowshower.GetComponent<Arrow_Shower_Collider>().damageForce = damageForce;
        arrowshower.transform.position = arrow_shower_startpoint.transform.position;
        arrowshower.SetActive(true);
        if (bow_shoot_sound[1] != null) bow_shoot_sound[1].PlayOneShot(bow_shoot_sound[1].clip);
    }
    protected override void Arrow_Shower_Startpoint()
    {
        arrow_shower_startpoint.SetActive(true);
    }
}
