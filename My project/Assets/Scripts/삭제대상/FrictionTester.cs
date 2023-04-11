using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionTester : MonoBehaviour
{
    private Rigidbody2D rb;
    private float baseSpeed = 100f;
    float movementX;
    private Vector3 m_Velocity = Vector3.zero;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        movementX = Input.GetAxisRaw("Horizontal") * baseSpeed;
    }
    private void FixedUpdate()
    {
        HorizontalMove();
    }
    private void HorizontalMove()
    {
        Vector3 targetVelocity;
        targetVelocity = new Vector2(movementX * Time.fixedDeltaTime, rb.velocity.y);

        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, 0.05f);
    }
}
