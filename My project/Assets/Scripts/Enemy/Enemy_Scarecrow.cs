using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Scarecrow : Enemy_Default
{
    private BoxCollider2D boxCollider2D;
    protected override void Start()
    {
        base.Start();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    public override void TakeDamage(float damage, Vector2 damageForce)
    {
        if (!animator.GetBool("IsDead"))
        {
            if (damage_sound.Length > 0)
            {
                int rand = Random.Range(0, damage_sound.Length);
                if (damage_sound[rand] != null) damage_sound[rand].PlayOneShot(damage_sound[rand].clip);
            }
            // 밀치는 힘이 있다면 타격 애니메이션
            if (damageForce.x != 0f) animator.SetTrigger("Hit");

            #region 데미지 텍스트 생성
            GameObject dmgText = DamageTextPool.Instance.GetFromPool();
            dmgText.transform.position = damagePoint.transform.position;
            dmgText.GetComponent<DamageText>().damage = damage;
            dmgText.SetActive(true);
            #endregion

            #region 타격 이펙트 생성
            GameObject hit_effect = HitFxPool.Instance.GetFromPool();
            hit_effect.transform.position = boxCollider2D.bounds.center;
            hit_effect.SetActive(true);
            #endregion

            curHP -= damage;
            healthbar.GetComponent<Enemy_Healthbar>().SetHealth(curHP, maxHP);
            if (curHP <= 0)
            {
                curHP = 0;
                Die();
            }
        }
    }
}
