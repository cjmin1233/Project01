using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{

    public Rigidbody2D myBody;
    public SpriteRenderer sr;
    public Animator animator;

    public float movementX;
    public float moveForce = 400f;
    public float jumpForce = 11f;

    private bool isGrounded;
    private bool isDead;

    //public CharacterController2D controller;

    //float horizontalMove = 0f;
    //int count = 1;
    //bool jump = false;

    public Transform player;
    private Vector3 tempPos;

    public float jumpRate = 0.5f;
    float nextJumpTime = 0f;
    
    void Update()
    {
        isDead = animator.GetBool("IsDead");
        if(!isDead)AnimateFrog();
    }
    void FixedUpdate()
    {
        if (!isDead)
        {
            tempPos = transform.position;
            if (tempPos.x - 10.0f <= player.position.x && player.position.x <= tempPos.x + 10.0f && Time.time >= nextJumpTime)
            {
                Debug.Log("detect player");
                FrogJump();
                nextJumpTime = Time.time + 1f / jumpRate;
            }

        }
    }
    void AnimateFrog()
    {
        if (movementX > 0)
        {
            sr.flipX = true;
        }
        else if (movementX < 0)
        {
            sr.flipX = false;
        }
    }
    void FrogJump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            if (tempPos.x > player.position.x) movementX = -1.0f;
            else movementX = 1.0f;

            //transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;

            myBody.AddForce(new Vector2(movementX * moveForce * Time.deltaTime, jumpForce), ForceMode2D.Impulse);
            animator.SetBool("IsJumping", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Frog Land");
            animator.SetBool("IsJumping", false);
        }
    }
}
