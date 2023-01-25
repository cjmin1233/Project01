using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Rigidbody2D rg;
    private Animator animator;
    public LayerMask enemyLayers;

    [SerializeField] private Transform firePoint;
    public GameObject ArrowPrefab;


    //private int SwordDamage = 40;
    public float swordDamage_z = 40;
    public float swordDamage_x = 40;
    public float swordDamage_z_multiplier = 1.0f;
    public float swordDamage_x_multiplier = 1.0f;


    // Combo attack ***************
    private int comboCounter = 0;
    public bool isZAttacking = false;
    public float Speed_Z = 1.0f;
    public GameObject[] comboCollider;
    public int inputZCounter = 0;
    public GameObject swordwindPrefab;
    public bool sword_wind_enable;
    public bool sword_storm_enable;
    public bool sword_cursed_enable;
    public bool bow_storm_enable;
    public bool bow_poison_enable;
    [SerializeField] private Transform sword_wind_startpoint;
    [SerializeField] private List<AudioSource> sword_wind_sound;
    [SerializeField] private AudioSource[] bow_shoot_sound;

    // ****************************

    // X attack *******************
    public bool isXAttacking = false;
    public float Speed_X = 1.0f;
    [SerializeField] private GameObject[] Sword_Collider_X;
    [SerializeField] private GameObject Bow_Beam;
    [SerializeField] private GameObject arrow_shower_startpoint;
    public bool sword_charging_enable;
    public bool sword_critical_enable;
    public bool sword_shield_enable;
    bool isCharging;
    private int chargeCounter = 0;
    [SerializeField] private AudioSource[] Charge_Sound;
    [SerializeField] private GameObject ChargeEffect;
    // ****************************
    int weaponType;
    bool isJumping;
    bool isDashing;

    private void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sword_wind_enable = false;
        sword_storm_enable = false;
        sword_cursed_enable = false;
        sword_charging_enable = false;
        sword_critical_enable = false;
        sword_shield_enable = false;
        bow_storm_enable = false;
        bow_poison_enable = false;
        isCharging = false;
        weaponType = animator.GetInteger("WeaponType");
    }
    void Update()
    {
        isDashing = animator.GetBool("IsDashing");
        isJumping = animator.GetBool("IsJumping");
        if ((sword_storm_enable || bow_storm_enable) && Input.GetButton("AttackZ") && !isZAttacking && !isJumping && !isXAttacking && !isDashing && comboCounter == 4)
        {
            if (weaponType == 1)
            {
                // Sword storm attack.
                SwordZAttack();
            }
            else if (weaponType == 2)
            {
                // Bow storm attack.
                BowZAttack();
            }
        }
        if (Input.GetButtonDown("AttackZ") && !isJumping && !isXAttacking && !isDashing && comboCounter<3)
        {
            if (isZAttacking)
            {
                // 스택 증가
                inputZCounter++;
                //Debug.Log("over Z input************");
            }
            else
            {
                if (weaponType == 1)
                {
                    //  Sword Z combo attack.
                    SwordZAttack();
                }
                else if (weaponType == 2)
                {
                    //  Bow Z combo attack.
                    BowZAttack();
                }
                else if (weaponType == 3)
                {
                    //  Dagger Z combo attack.
                    DaggerZAttack();
                }
            }
        }

        if (Input.GetButtonDown("AttackX") && !isXAttacking && !isZAttacking && !isJumping && !isDashing)
        {
            // Z+X 콤보
            if (comboCounter == 3 && (weaponType == 1 || weaponType == 2))
            {
                gameObject.GetComponent<Player>().canMove = false;
                if (weaponType == 2)
                {
                    arrow_shower_startpoint.GetComponent<Arrow_Shower_Startpoint>().anim_Speed = Speed_X;
                }
                animator.SetFloat("Speed_X", Speed_X);
                isXAttacking = true;
                animator.SetTrigger("Combo3");

                if (weaponType == 1)
                {
                    float swordCombo_force = 0.1f;
                    if (transform.rotation.y != 0f) swordCombo_force *= -1f;
                    rg.AddForce(new Vector2(swordCombo_force, 0f), ForceMode2D.Impulse);
                }
            }
            // 일반 X
            else 
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
                    DaggerXAttack();
                }
            }
        }
        if(Input.GetButtonUp("AttackX") && isXAttacking && !isZAttacking && !isJumping && !isDashing && isCharging)
        {
            if (weaponType == 1)
            {
                isCharging = false;
                animator.SetBool("IsCharging", isCharging);
            }
        }

        //  over Z input handle
        if (!isZAttacking && !isJumping && !isXAttacking && !isDashing && comboCounter < 3 && inputZCounter > 0)
        {
            if (weaponType == 1)
            {
                //  Sword Z combo attack.
                SwordZAttack();
                //inputZCounter=0;
            }
            else if (weaponType == 2)
            {
                //  Bow Z combo attack.
                BowZAttack();
                //inputZCounter = 0;
            }
            else if (weaponType == 3)
            {
                //  Dagger Z combo attack.
                DaggerZAttack();
            }
            inputZCounter = 0;
        }

    }
    private void Start_Combo()
    {
        if (comboCounter == 1 && (sword_storm_enable || bow_storm_enable)) comboCounter = 4;
        else if (comboCounter < 3)
        {
            comboCounter++;
        }
        isZAttacking = false;
        //Enable_Sword_Combo_Collider();
    }
    private void Finish_Combo()
    {
        inputZCounter = 0;
        comboCounter = 0;
        isZAttacking = false;
        gameObject.GetComponent<Player>().canMove = true;
        // 스택 초기화
    }
    private void Finish_X()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().playerFollowing = true;

        isXAttacking = false;
        comboCounter = 0;
        gameObject.GetComponent<Player>().canMove = true;
        // 스택 초기화
        inputZCounter = 0;
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

        comboCollider[comboCounter].GetComponent<Combo_Collider>().damage = Mathf.Round(swordDamage_z * swordDamage_z_multiplier * (1 + (float)comboCounter * 0.2f));
        comboCollider[comboCounter].GetComponent<Combo_Collider>().damageForce = damageForce;
    }
    private void ShootSwordWind()
    {
        if (sword_wind_enable)
        {
            GameObject swordwind = SwordWindPool.Instance.GetFromPool();

            swordwind.GetComponent<Sword_Wind_Collider>().damage = Mathf.Round(0.5f * swordDamage_z * swordDamage_z_multiplier * (1 + (float)comboCounter * 0.2f));
            swordwind.GetComponent<Sword_Wind_Collider>().anim_Speed = Speed_Z;

            swordwind.transform.position = sword_wind_startpoint.position;
            swordwind.SetActive(true);
            int rand = Random.Range(0, sword_wind_sound.Count);
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
        /*//Debug.Log("i'm here");
        float swordCombo_force = 10f;
        if (transform.rotation.y != 0f) swordCombo_force *= -1f;*/
        rg.AddForce(new Vector2(transform.right.x * 10f, 0f), ForceMode2D.Impulse);
        //rg.AddForce(new Vector2(swordCombo_force, 0f), ForceMode2D.Force);
    }
    private void Enable_Sword_Collider_X()
    {
        Vector2 damageForce = new Vector2(transform.right.x * 20f, 0f);
        float damage = 0f;
        for(int i = 0; i < Sword_Collider_X.Length; i++)
        {
            damage = Mathf.Round(swordDamage_x * swordDamage_x_multiplier * (1 + 0.5f * (float)chargeCounter));
            if (sword_critical_enable)
            {
                int rand = Random.Range(1, 101);
                if (rand <= 40)
                {
                    Sword_Collider_X[i].GetComponent<Combo_Collider>().critical = true;
                    damage = Mathf.Round(damage * 1.5f);
                }
                else Sword_Collider_X[i].GetComponent<Combo_Collider>().critical = false;
            }
            Sword_Collider_X[i].GetComponent<Combo_Collider>().damage = damage;
            Sword_Collider_X[i].GetComponent<Combo_Collider>().damageForce = damageForce;
        }
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

        float swordCombo_force = 0.1f;
        if (transform.rotation.y != 0f) swordCombo_force *= -1f;
        rg.AddForce(new Vector2(swordCombo_force, 0f), ForceMode2D.Impulse);
    }
    private void BowZAttack()
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
        }
    }
    private void ShootArrow()
    {
        GameObject arrow = ArrowPool.Instance.GetFromPool();
        arrow.GetComponent<Bullet>().damage = Mathf.Round(40f * (1 + comboCounter * 0.2f));
        arrow.GetComponent<Bullet>().anim_Speed = Speed_Z;
        if (bow_poison_enable) arrow.GetComponent<Bullet>().isPoisoned = true;
        arrow.transform.position = firePoint.position;
        arrow.SetActive(true);
        /*int rand = Random.Range(0, sword_wind_sound.Count);*/
        if (bow_shoot_sound[0] != null) bow_shoot_sound[0].PlayOneShot(bow_shoot_sound[0].clip);
    }
    private void BowXAttack()
    {
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;

        animator.SetFloat("Speed_X", Speed_X);
        isXAttacking = true;
        animator.SetTrigger("AttackX");
        isZAttacking = false;
        comboCounter = 0;
        //Instantiate(ArrowPrefab, firePoint.position, firePoint.rotation);
    }
    private void Enable_Bow_Beam()
    {
        Vector2 damageForce = new Vector2(transform.right.x * 10f, 0f);
        //if (transform.rotation.y != 0f) damageForce.x *= -1f;
        Bow_Beam.GetComponent<Bow_Beam_Collider>().damage = Mathf.Round(111f);
        Bow_Beam.GetComponent<Bow_Beam_Collider>().anim_Speed = Speed_X;
        Bow_Beam.GetComponent<Bow_Beam_Collider>().damageForce = damageForce;
    }
    private void Enable_Arrow_Shower()
    {
        Vector2 damageForce = new Vector2(transform.right.x * 8f, 0f);
        GameObject arrowshower = ArrowShowerPool.Instance.GetFromPool();
        arrowshower.GetComponent<Arrow_Shower_Collider>().damage = Mathf.Round(11f);
        arrowshower.GetComponent<Arrow_Shower_Collider>().damageForce = damageForce;
        arrowshower.transform.position = arrow_shower_startpoint.transform.position;
        arrowshower.SetActive(true);
        if (bow_shoot_sound[1] != null) bow_shoot_sound[1].PlayOneShot(bow_shoot_sound[1].clip);
    }
    private void Arrow_Shower_Startpoint()
    {
        arrow_shower_startpoint.SetActive(true);
    }

    // Hashashin functions
    private void DaggerZAttack()
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
            //animator.ResetTrigger("Combo3");
        }
    }
    private void Enable_Dagger_Combo_Collider()
    {
        Vector2 damageForce = new Vector2(0.1f * transform.right.x, 0f);
        //if (transform.rotation.y != 0f) damageForce.x *= -1f;
        comboCollider[comboCounter].GetComponent<Combo_Collider>().damage = Mathf.Round(55f * (1 + comboCounter * 0.2f));
        comboCollider[comboCounter].GetComponent<Combo_Collider>().damageForce = damageForce;
    }
    private void DaggerXAttack()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().playerFollowing = false;
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;

        animator.SetFloat("Speed_X", Speed_X);
        isXAttacking = true;
        // after image on
        gameObject.GetComponent<Player>().AfterImageAvailable = true;
        animator.SetTrigger("AttackX");
        isZAttacking = false;
        comboCounter = 0;
    }

    private void Enable_Dagger_X_Collider()
    {
        Vector2 damageForce = new Vector2(1f * transform.right.x, 0f);
        //if (transform.rotation.y != 0f) damageForce.x *= -1f;
        for (int i = 0; i < Sword_Collider_X.Length; i++)
        {
            Sword_Collider_X[i].GetComponent<Combo_Collider>().damage = Mathf.Round(77f);
            Sword_Collider_X[i].GetComponent<Combo_Collider>().damageForce = damageForce;
        }
    }

    public void PlayerInit()
    {
        // after image off
        gameObject.GetComponent<Player>().AfterImageAvailable = false;
        GetComponent<Player>().canMove = true;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().playerFollowing = true;

        isZAttacking = false;
        isXAttacking = false;
        comboCounter = 0;
        inputZCounter = 0;
    }
}
