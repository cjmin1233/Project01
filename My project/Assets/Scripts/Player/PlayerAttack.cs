using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public bool isZAttacking = false;
    public bool isXAttacking = false;
    public float Speed_Z = 1.0f;
    public float Speed_X = 1.0f;
    public Dictionary<string, float> speed_z_buffer = new Dictionary<string, float>();
    public Dictionary<string, float> speed_x_buffer = new Dictionary<string, float>();

    protected Rigidbody2D rb;
    protected Animator animator;

    protected int comboCounter = 0;
    protected int inputZCounter = 0;

    public float playerPower = 100f;
    //public float damage_z_multiplier = 1.0f;
    //public float damage_x_multiplier = 1.0f;

    public Dictionary<string, float> power_buffer = new Dictionary<string, float>();
    public Dictionary<string, float> damage_z_buffer = new Dictionary<string, float>();
    public Dictionary<string, float> damage_x_buffer = new Dictionary<string, float>();


    public bool sword_wind_enable;
    public bool sword_storm_enable;
    public bool sword_cursed_enable;
    public bool sword_charging_enable;
    public bool sword_critical_enable;
    public bool sword_shield_enable;

    public bool bow_storm_enable;
    public bool bow_poison_enable;
    public bool bow_air_enable;
    public bool bow_rain_enable;
    public bool bow_slow_enable;
    public bool bow_fast_enable;

    public bool dagger_storm_enable;
    public bool quick_wind_enable;
    public bool assassin_enable;

    protected bool isJumping;
    protected bool isDashing;

    //int weaponType;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //weaponType = animator.GetInteger("WeaponType");

        sword_wind_enable = false;
        sword_storm_enable = false;
        sword_cursed_enable = false;
        sword_charging_enable = false;
        sword_critical_enable = false;
        sword_shield_enable = false;

        bow_storm_enable = false;
        bow_poison_enable = false;
        bow_air_enable = false;
        bow_rain_enable = false;
        bow_slow_enable = false;
        bow_fast_enable = false;

        dagger_storm_enable = false;
        quick_wind_enable = false;
        assassin_enable = false;

        isJumping = false;
        isDashing = false;
    }
    protected virtual void Update() { }
    private void Start_Combo()
    {
        if (comboCounter == 1 && (sword_storm_enable || bow_storm_enable || dagger_storm_enable)) comboCounter = 4;
        else if (comboCounter < 3)
        {
            comboCounter++;
        }
        isZAttacking = false;
        animator.SetBool("IsZAttacking", isZAttacking);
    }
    private void Finish_Combo()
    {
        inputZCounter = 0;
        comboCounter = 0;
        isZAttacking = false;
        animator.SetBool("IsZAttacking", isZAttacking);

        gameObject.GetComponent<Player>().canMove = true;
        // 스택 초기화
    }
    private void Finish_X()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().playerFollowing = true;

        isXAttacking = false;
        animator.SetBool("IsXAttacking", isXAttacking);

        comboCounter = 0;
        gameObject.GetComponent<Player>().canMove = true;
        // 스택 초기화
        inputZCounter = 0;
    }

    public void PlayerInit()
    {
        // after image off
        gameObject.GetComponent<Player>().AfterImageAvailable = false;
        GetComponent<Player>().canMove = true;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().playerFollowing = true;

        isZAttacking = false;
        isXAttacking = false;
        animator.SetBool("IsZAttacking", isZAttacking);
        animator.SetBool("IsXAttacking", isXAttacking);

        comboCounter = 0;
        inputZCounter = 0;
    }
    protected float Z_SpeedCalculation()
    {
        float z_multiplier = 1f;
        foreach (float value in speed_z_buffer.Values)
        {
            z_multiplier += value;
        }

        return z_multiplier;
    }
    protected float X_SpeedCalculation()
    {
        float x_multiplier = 1f;
        foreach (float value in speed_x_buffer.Values)
        {
            x_multiplier += value;
        }

        return x_multiplier;
    }
    protected float PlayerPowerCalculation()
    {
        float full_power = 1f;
        foreach (float value in power_buffer.Values)
        {
            full_power += value;
        }

        return playerPower * full_power;
    }
    protected float Z_DamageCalculation()
    {
        float z_multiplier = 1f;
        foreach (float value in damage_z_buffer.Values)
        {
            z_multiplier += value;
        }

        return z_multiplier;
    }
    protected float X_DamageCalculation()
    {
        float x_multiplier = 1f;
        foreach (float value in damage_x_buffer.Values)
        {
            x_multiplier += value;
        }

        return x_multiplier;
    }
}
