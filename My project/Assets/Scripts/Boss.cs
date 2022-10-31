using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    GameObject player;
    [SerializeField] private GameObject fireWall01;
    [SerializeField] private GameObject fireWall02;
    SpriteRenderer sr;
    Rigidbody2D rb;
    Animator animator;

    private float movementX = 200f;

    //private bool isFacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;
    private float m_MovementSmoothing = 0.05f;

    private bool isActing;
    private float waitTime = 2f;
    private float timer = 0f;
    private int nextAction;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        isActing = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isActing) timer += Time.deltaTime;
        if (timer > waitTime)
        {
            int random = Random.Range(0, 2);
            nextAction = random;
            if (player.transform.position.x < gameObject.transform.position.x) movementX = -200.0f;
            else movementX = 200f;

            isActing = true;
            timer = 0f;
        }

        Flip();
    }

    private void FixedUpdate()
    {
        if (isActing)
        {
            if (nextAction == 0)
            {
                BossMove();
            }

            else if (nextAction == 1)
            {
                animator.SetBool("Skill_FireWall", true);
                //fireWall01.gameObject.SetActive(true);
            }
        }
    }
    private void Fin_Act()
    {
        isActing = false;
        animator.SetBool("Skill_FireWall", false);
        rb.velocity = Vector3.zero;
        animator.SetFloat("Speed", 0);
    }
    private void BossMove()
    {
        Vector3 targetVelocity;

        targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        animator.SetFloat("Speed", Mathf.Abs(movementX));
    }

    private void FireWall01()
    {
        fireWall01.gameObject.SetActive(true);
    }
    private void FireWall02()
    {
        fireWall02.gameObject.SetActive(true);
    }
    private void Flip()
    {
        if (movementX >= 0)
        {
            //isFacingRight = true;
            if (transform.rotation.y != 0f) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            //isFacingRight = false;
            if (transform.rotation.y == 0f) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

}
