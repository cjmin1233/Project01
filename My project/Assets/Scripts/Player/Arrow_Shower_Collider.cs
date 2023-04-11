using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Shower_Collider : MonoBehaviour
{
    public int fxType;
    [HideInInspector] public float damage;
    [HideInInspector] public Vector2 damageForce;
    [HideInInspector] public bool rain_enable = false;
    [HideInInspector] public bool slow_enable = false;
    private float damage_multiplier = 1f;
    //private List<string> hit_list;
    private void OnEnable()
    {
        damage_multiplier = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        float curDamage = Mathf.Round(damage * damage_multiplier);
        if (tag == "Enemy" || tag == "Boss")
        {
            collision.GetComponent<Enemy_Default>().TakeDamage(curDamage, damageForce, false, Color.green, fxType);
            if (damage_multiplier < 1.6f && rain_enable) damage_multiplier += 0.1f;
            if (slow_enable) collision.GetComponent<Enemy_Default>().Debuff("Slow", 5f);
        }
    }
    private void Disable_Collider()
    {
        ArrowShowerPool.Instance.AddToPool(gameObject);
    }
}
