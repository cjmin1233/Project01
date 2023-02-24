using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Collider_Checker : MonoBehaviour
{
    int ground_layer;
    [HideInInspector] public bool groundCollision;
    private void Awake()
    {
        ground_layer = LayerMask.NameToLayer("Ground");
        groundCollision = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ground_layer == collision.gameObject.layer)
        {
            //Debug.Log("ground collision enter");
            groundCollision = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (ground_layer == collision.gameObject.layer)
        {
            //Debug.Log("ground collision exit");
            groundCollision = false;
        }
    }
}
