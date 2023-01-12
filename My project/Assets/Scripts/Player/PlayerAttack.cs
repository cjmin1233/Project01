using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Rigidbody2D rg;
    private Animator animator;
    //private SpriteRenderer sr;
    //public Transform SwordPoint;
    //public float SwordRange = 0.5f;
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
    public int inputZCounter = 0;
    public GameObject swordwindPrefab;
    public GameObject[] swordwindCollider;
    public bool sword_wind_enable;
    [SerializeField] private Transform sword_wind_startpoint;
    [SerializeField] private List<AudioSource> sword_wind_sound;
    [SerializeField] private AudioSource bow_shoot_sound;

    // ****************************

    // X attack *******************
    public bool isXAttacking = false;
    public float Speed_X = 1.0f;
    public GameObject Sword_Collider_X;
    [SerializeField] private GameObject Bow_Beam;
    [SerializeField] private GameObject arrow_shower_startpoint;

    // ****************************
    int weaponType;
    bool isJumping;
    bool isDashing;

    //public float swordCombo_force = 100f;

    private void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        //sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sword_wind_enable = false;
        weaponType = animator.GetInteger("WeaponType");
    }
    void Update()
    {
        isDashing = animator.GetBool("IsDashing");
        isJumping = animator.GetBool("IsJumping");

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
                    //  Spear Z combo attack.
                }
            }
        }

        if (Input.GetButtonDown("AttackX") && !isXAttacking && !isZAttacking && !isJumping && !isDashing)
        {
            // Z+X 콤보
            if (comboCounter == 3)
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
                }
            }
        }

        //  over Z input handle
        if (!isZAttacking && !isJumping && !isXAttacking && !isDashing && comboCounter < 3 && inputZCounter > 0)
        {
            if (weaponType == 1)
            {
                //  Sword Z combo attack.
                SwordZAttack();
                inputZCounter=0;
            }
            else if (weaponType == 2)
            {
                //  Bow Z combo attack.
                BowZAttack();
                inputZCounter = 0;
            }
            else if (weaponType == 3)
            {
                //  Spear Z combo attack.
            }
        }

    }
    public void Start_Combo()
    {
        if (comboCounter < 3)
        {
            comboCounter++;
        }
        isZAttacking = false;
        //Enable_Sword_Combo_Collider();
    }
    public void Finish_Combo()
    {
        inputZCounter = 0;
        comboCounter = 0;
        isZAttacking = false;
        gameObject.GetComponent<Player>().canMove = true;
        // 스택 초기화
    }
    public void Finish_X()
    {
        isXAttacking = false;
        comboCounter = 0;
        gameObject.GetComponent<Player>().canMove = true;
        // 스택 초기화
        inputZCounter = 0;
    }

    private void Enable_Sword_Combo_Collider()
    {
        Vector2 damageForce = new Vector2(20f,0f);
        if (transform.rotation.y != 0f) damageForce.x *= -1f;
        comboCollider[comboCounter].GetComponent<Sword_Combo_Collider>().damage = Mathf.Round(swordDamage_z * swordDamage_z_multiplier * (1+comboCounter*0.2f));
        comboCollider[comboCounter].GetComponent<Sword_Combo_Collider>().anim_Speed = Speed_Z;
        comboCollider[comboCounter].GetComponent<Sword_Combo_Collider>().damageForce = damageForce;
        comboCollider[comboCounter].SetActive(true);
        if (sword_wind_enable)
        {
            GameObject swordwind = SwordWindPool.Instance.GetFromPool();
            //swordwind.SetActive(false);

            swordwind.GetComponent<Sword_Wind_Collider>().damage = Mathf.Round(0.5f * swordDamage_z * swordDamage_z_multiplier * (1 + comboCounter * 0.2f));
            swordwind.GetComponent<Sword_Wind_Collider>().anim_Speed = Speed_Z;
            swordwind.GetComponent<Sword_Wind_Collider>().damageForce = damageForce;
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
        }
        // 공격시 약 전진
        //Debug.Log("i'm here");
        float swordCombo_force = 20f;
        if (transform.rotation.y != 0f) swordCombo_force *= -1f;
        rg.AddForce(new Vector2(swordCombo_force, 0f), ForceMode2D.Impulse);
        //rg.AddForce(new Vector2(swordCombo_force, 0f), ForceMode2D.Force);
    }
    private void Enable_Sword_Collider_X()
    {
        Vector2 damageForce = new Vector2(transform.right.x * 20f, 0f);
        //if (transform.rotation.y != 0f) damageForce.x *= -1f;
        Sword_Collider_X.GetComponent<Sword_Combo_Collider>().damage = Mathf.Round(swordDamage_x * swordDamage_x_multiplier);
        Sword_Collider_X.GetComponent<Sword_Combo_Collider>().anim_Speed = Speed_X;
        Sword_Collider_X.GetComponent<Sword_Combo_Collider>().damageForce = damageForce;
        Sword_Collider_X.SetActive(true);
    }
    private void SwordXAttack()
     {
        // 공격동안 움직임 제어
        gameObject.GetComponent<Player>().canMove = false;

        animator.SetFloat("Speed_X", Speed_X);
        isXAttacking = true;
        animator.SetTrigger("AttackX");
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
        arrow.transform.position = firePoint.position;
        arrow.SetActive(true);
        /*int rand = Random.Range(0, sword_wind_sound.Count);*/
        if (bow_shoot_sound != null) bow_shoot_sound.PlayOneShot(bow_shoot_sound.clip);
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
        Bow_Beam.SetActive(true);
    }
    private void Enable_Arrow_Shower()
    {
        Vector2 damageForce = new Vector2(transform.right.x * 8f, 0f);
        GameObject arrowshower = ArrowShowerPool.Instance.GetFromPool();
        arrowshower.GetComponent<Arrow_Shower_Collider>().damage = Mathf.Round(11f);
        //swordwind.GetComponent<Sword_Wind_Collider>().anim_Speed = Speed_Z;
        arrowshower.GetComponent<Arrow_Shower_Collider>().damageForce = damageForce;
        arrowshower.transform.position = arrow_shower_startpoint.transform.position;
        arrowshower.SetActive(true);
/*        int rand = Random.Range(0, sword_wind_sound.Count);
        if (sword_wind_sound[rand] != null) sword_wind_sound[rand].PlayOneShot(sword_wind_sound[rand].clip);
*/
    }
    private void Arrow_Shower_Startpoint()
    {
        arrow_shower_startpoint.SetActive(true);
    }
    public void PlayerInit()
    {
        isZAttacking = false;
        isXAttacking = false;
        comboCounter = 0;
        inputZCounter = 0;
    }
}
