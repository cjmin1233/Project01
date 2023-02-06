using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Fly : Enemy_Default
{
    private float movementY;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;
    protected override void Update()
    {
        // 체력바 이동
        healthbar.GetComponent<Slider>().transform.position = Camera.main.WorldToScreenPoint(transform.position + Offset);
        if (!animator.GetBool("IsDead"))
        {
            if (!detectPlayer) canMove = false;
            else
            {
                for (int i = 0; i < range.Length; i++)
                {
                    if (range[i])
                    {
                        canMove = false;
                        if (Time.time >= lastAttackTime + 1 / attackRate)
                        {
                            LookPlayer();
                            Attack(i);
                            lastAttackTime = Time.time;
                        }
                        break;
                    }
                }
                if (!isAttacking && !range[1]) canMove = true;
            }

            currentSpeed = moveSpeed_multiplier * baseSpeed;
            if (canMove)
            {
                movementX = currentSpeed;
                movementY = currentSpeed;
                LookPlayer();
            }
            else
            {
                movementX = 0f;
                movementY = 0f;
            }
            animator.SetFloat("Speed", Mathf.Abs(movementX));

            Flip();
        }
    }
    /*protected override void FixedUpdate()
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
                        //InRange = true;
                        movementX = 0;
                        //movementY = 0;
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
    }*/
    protected override void LookPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerCenter = player.GetComponent<BoxCollider2D>().bounds.center;
        float dist_x = Mathf.Abs(playerCenter.x - transform.position.x);
        float dist_y = Mathf.Abs(playerCenter.y - transform.position.y);


        if ((playerCenter.x < transform.position.x && movementX > 0) || (transform.position.x < playerCenter.x && movementX < 0)) movementX *= -1f;
        if ((playerCenter.y < transform.position.y && movementY > 0) || (transform.position.y < playerCenter.y && movementY < 0)) movementY *= -1f;
        if (dist_x <= 0.1f) movementX = 0;
        if (dist_y <= 0.1f) movementY = 0;
    }
    protected override void Move()
    {
        Vector3 targetVelocity;

        targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, movementY * Time.fixedDeltaTime);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }
    private void Shoot()
    {
        GameObject gameObject = EnemyBulletPool.Instance.GetFromPool();
        gameObject.GetComponent<Enemy_Bullet>().damage = 10f;
        gameObject.transform.position = firePoint.position;
        gameObject.transform.rotation = transform.rotation;
        gameObject.SetActive(true);
    }
}
