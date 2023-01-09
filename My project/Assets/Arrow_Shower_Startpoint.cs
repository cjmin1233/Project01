using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Shower_Startpoint : MonoBehaviour
{
    public Transform offset;
    [HideInInspector] public float anim_Speed;

    private List<string> hit_list;
    private Rigidbody2D rb;
    private float speed = 20f;
    private void Awake()
    {
        hit_list = new List<string>();
        //offset = GetComponent<Transform>().position;
        rb = GetComponent<Rigidbody2D>();
        anim_Speed = 1f;
    }
    private void OnEnable()
    {
        hit_list.Clear();
        transform.position = offset.position;
        Debug.Log("position is : " + transform.position.x);
        rb.velocity = transform.right * speed * anim_Speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;
        if (tag == "Enemy" || tag == "Boss")
        {
            Debug.Log("Found target");
            rb.velocity = Vector2.zero;
            //float offset_X = collision.GetComponent<Transform>().position.x - transform.position.x;
            //transform.Translate(new Vector3(offset_X, 0f, 0f));
            //ArrowShowerPool.Instance.Init_Startpoint(collision.GetComponent<Transform>());
            //gameObject.SetActive(false);
            //transform.position.x = collision.gameObject.transform.position.x;

        }
    }
}
