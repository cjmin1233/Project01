using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Ground_Checker : MonoBehaviour
{
    int ground_layer;
    [HideInInspector] public bool isGrounded;
    private void Awake()
    {
        ground_layer = LayerMask.NameToLayer("Ground");
        isGrounded = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ground_layer == collision.gameObject.layer)
        {
            //Debug.Log("ground landing");
            isGrounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (ground_layer == collision.gameObject.layer)
        {
            //Debug.Log("ground landing");
            isGrounded = false;
        }
    }
}
