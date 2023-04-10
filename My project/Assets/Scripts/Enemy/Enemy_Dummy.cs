using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy_Dummy : Enemy_Default
{
    //private float dpsCheck_StartTime;
    private float dpsCheck_TotalDamage = 0f;
    private float lastDamageTime = 0f;
    private float lastDamage;
    private float dpsCheck_Timer;
    private float dps;
    private bool IsDpsChecking = false;
    [SerializeField] private float dpsCheck_ResetTime;
    [SerializeField] private GameObject dpsChecker;
    private TextMeshPro dpsText;
    private RectTransform dpsText_rect;
    private string lastDamageText;
    private string DpsText;
    private string totalDamageText;

    protected override void OnEnable()
    {
        base.OnEnable();
        dpsText = dpsChecker.GetComponent<TextMeshPro>();
        dpsText_rect = dpsChecker.GetComponent<RectTransform>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    protected override void Update()
    {
        // ü�¹� �̵�
        healthbar.GetComponent<Slider>().transform.position = Camera.main.WorldToScreenPoint(transform.position + Offset);
        if (!animator.GetBool("IsDead"))
        {
            DebuffChecker();
            DpsCheck();
            if (!detectPlayer || player.GetComponent<Player>().isDead) canMove = false;
            else
            {
                for (int i = 0; i < range.Length; i++)
                {
                    if (range[i])
                    {
                        canMove = false;
                        if (Time.time >= lastAttackTime + 1 / attackRate && !isAttacking)
                        {
                            LookPlayer();
                            Attack(i);
                            lastAttackTime = Time.time;
                        }
                        break;
                    }
                }
            }
            currentSpeed = moveSpeed_multiplier * baseSpeed;
            if (canMove)
            {
                movementX = currentSpeed;
                LookPlayer();
            }
            else
            {
                movementX = 0f;
            }
            animator.SetFloat("Speed", Mathf.Abs(movementX));
        }
    }

    public override void TakeDamage(float damage, Vector2 damageForce, bool isCrit, Color damageColor, int fxType)
    {
        if (!animator.GetBool("IsDead"))
        {
            if (damage_sound.Length > 0)
            {
                int rand = Random.Range(0, damage_sound.Length);
                if (damage_sound[rand] != null) damage_sound[rand].PlayOneShot(damage_sound[rand].clip);
            }

            // ��ġ�� ���� �ִٸ� Ÿ�� �ִϸ��̼�
            if (Mathf.Abs(damageForce.x) > 0.01f) animator.SetTrigger("Hit");
            #region ������ �ؽ�Ʈ ����
            GameObject dmgText = DamageTextPool.Instance.GetFromPool();
            dmgText.transform.position = damagePoint.transform.position;
            dmgText.GetComponent<DamageText>().damage = damage;
            dmgText.GetComponent<DamageText>().x_dir = damageForce.normalized.x;
            dmgText.GetComponent<DamageText>().textColor = damageColor;
            dmgText.SetActive(true);
            #endregion

            #region Ÿ�� ����Ʈ ����
            if (fxType >= 0)
            {
                GameObject hit_effect = HitFxPool.Instance.GetFromPool();
                float x_rand = 0.5f * Random.Range((-1f) * boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.x);
                float y_rand = 0.5f * Random.Range((-1f) * boxCollider2D.bounds.extents.y, boxCollider2D.bounds.extents.y);
                Vector3 temp = new Vector3(boxCollider2D.bounds.center.x + x_rand, boxCollider2D.bounds.center.y + y_rand, 0);
                hit_effect.transform.position = temp;
                hit_effect.GetComponent<HitFx>().fxType = fxType;
                hit_effect.SetActive(true);
            }
            #endregion

            curHP -= damage;
            if (curHP < 1f) curHP = 1f;
            healthbar.GetComponent<Enemy_Healthbar>().SetHealth(curHP, maxHP);
            /*if (recovery > 0f && !isRecovering)
            {
                StartCoroutine(AutoRecovery());
            }*/
            // dps ����
            IsDpsChecking = true;
            lastDamage = damage;
            dpsCheck_TotalDamage += damage;
            lastDamageTime = Time.time;
        }
    }
    protected override void Die()
    {
        //
    }
    private void DpsCheck()
    {
        if (IsDpsChecking)
        {
            dpsCheck_Timer += Time.deltaTime;
            dps = Mathf.Round(dpsCheck_TotalDamage / dpsCheck_Timer * 10f) / 10f;
            dpsCheck_TotalDamage = Mathf.Round(dpsCheck_TotalDamage * 10f) / 10f;

            lastDamageText = "������ �ϰ� : " + Mathf.RoundToInt(lastDamage).ToString() + "\n";
            DpsText = "�ʴ� ���ط� : " + dps.ToString() + "\n";
            totalDamageText = "       �հ� : " + dpsCheck_TotalDamage.ToString();

            dpsText.text = lastDamageText + DpsText + totalDamageText;
            //dpsText_rect.rotation = Quaternion.Euler(0f, 0f, 0f);
            dpsChecker.transform.localRotation = transform.rotation;
            // ���� �ð� ������ �� dps üŷ ����
            if (Time.time > lastDamageTime + dpsCheck_ResetTime)
            {
                IsDpsChecking = false;
                // ȸ��
                curHP = maxHP;
                healthbar.GetComponent<Enemy_Healthbar>().SetHealth(curHP, maxHP);

                // dps ���� �� ����
                dpsCheck_Timer = 0f;
                dpsCheck_TotalDamage = 0f;
                dpsText.text = "";
            }
        }
    }
}
