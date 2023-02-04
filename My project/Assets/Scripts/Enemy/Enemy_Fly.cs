using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Fly : Enemy_Default
{
    private bool InRange = false;
    private float movementY;
    protected override void FixedUpdate()
    {
        // 체력바 이동
        healthbar.GetComponent<Slider>().transform.position = Camera.main.WorldToScreenPoint(transform.position + Offset);

        if(!isAttacking && !animator.GetBool("IsDead"))
        {
            if (!detectPlayer) Stop();
            else
            {
                //InRange = false;
                // 플레이어 발견
                player = GameObject.FindGameObjectWithTag("Player");

                for(int i = 0; i < range.Length; i++)
                {
                    if (range[i])
                    {
                        InRange = true;
                        movementX = 0;
                        movementY = 0;
                        if (Time.time >= lastAttackTime + 1 / attackRate)
                        {
                            Fly_Attack(i);
                            lastAttackTime = Time.time;
                        }
                        break;
                    }
                }
                if (!range[1] && !isAttacking)
                {
                    Vector3 playerCenter = player.GetComponent<BoxCollider2D>().bounds.center;
                    // 플레이어 발견했지만 공격범위 아닐 때
                    movementX = baseSpeed;
                    movementY = baseSpeed;
                    float dist_x = Mathf.Abs(playerCenter.x - transform.position.x);
                    float dist_y = Mathf.Abs(playerCenter.y - transform.position.y);


                    if ((playerCenter.x < transform.position.x && movementX > 0) || (transform.position.x < playerCenter.x && movementX < 0)) movementX *= -1f;
                    if ((playerCenter.y < transform.position.y && movementY > 0) || (transform.position.y < playerCenter.y && movementY < 0)) movementY *= -1f;
                    if (dist_x <= 0.1f) movementX = 0;
                    if (dist_y <= 0.1f) movementY = 0;
                }
                Move(); 
            }
        }
        Flip();
    }
    protected override void Move()
    {
        //transform.position = Vector2.MoveTowards(transform.position, player.transform.position, baseSpeed * Time.deltaTime);
        Vector3 targetVelocity;

        targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, movementY * Time.fixedDeltaTime);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        animator.SetFloat("Speed", Mathf.Abs(movementX));

    }
    private void Fly_Attack(int idx)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //if ((player.transform.position.x < transform.position.x && movementX > 0) || (transform.position.x < player.transform.position.x && movementX < 0)) movementX *= -1f;

        animator.SetTrigger("Attack" + idx);
        isAttacking = true;
        animator.SetBool("IsAttacking", isAttacking);

        //Stop();
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
