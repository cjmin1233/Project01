using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Fly : Enemy_Default
{
    private bool InRange = false;
    protected override void FixedUpdate()
    {
        // 체력바 이동
        healthbar.GetComponent<Slider>().transform.position = Camera.main.WorldToScreenPoint(transform.position + Offset);

        if(!isAttacking && !animator.GetBool("IsDead"))
        {
            if (!detectPlayer) Stop();
            else
            {
                InRange = false;
                for(int i = 0; i < range.Length; i++)
                {
                    if (range[i] && Time.time >= lastAttackTime + 1 / attackRate)
                    {
                        InRange = true;
                        Fly_Attack(i);
                        lastAttackTime = Time.time;
                    }
                }
                if (!InRange)
                {
                    movementX = baseSpeed;
                    player = GameObject.FindGameObjectWithTag("Player");
                    if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;
                    Move();
                }
            }
            

        }
        Flip();
    }
    private void Fly_Attack(int idx)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;

        animator.SetTrigger("Attack" + idx);
        isAttacking = true;
        animator.SetBool("IsAttacking", isAttacking);

        Stop();
    }
    private void Dash_1()
    {
        Vector2 force = new Vector2(transform.right.x * 5f, 0f);
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    private void Dash_2()
    {
        Vector2 force = new Vector2(transform.right.x * 10f, 0f);
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
