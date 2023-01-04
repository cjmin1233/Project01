using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Wind_Collider : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float anim_Speed;
    [HideInInspector] public Vector2 damageForce;
    //public Transform start_point;
    private Transform player;

    //public List<AudioSource> audioSource;
    private Animator animator;
    private Rigidbody2D rb;
    private List<string> hit_list;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        hit_list = new List<string>();
        //damageForce = Vector2.zero;
        anim_Speed = 1.0f;
    }
    private void OnEnable()
    {
        //
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = player.position;
        transform.rotation = player.rotation;
        //
        animator.SetFloat("EnableSpeed", anim_Speed);
/*        int rand = Random.Range(0, audioSource.Count);
        if (audioSource[rand] != null) audioSource[rand].PlayOneShot(audioSource[rand].clip);
*/        
        Vector2 vel = new Vector2(10f * anim_Speed, 0f);
        if (damageForce.x < 0) vel.x *= -1f;
        rb.velocity = vel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;
        if (!hit_list.Contains(name))
        {
            hit_list.Add(name);
            if (tag == "Enemy")
            {
                //Debug.Log("hit " + collision.name);
                collision.GetComponent<Enemy>().TakeDamage(damage, Vector2.zero);
            }
            else if (tag == "Boss")
            {
                collision.GetComponent<Boss>().TakeDamage(damage);
            }
            //else Debug.Log("We hit " + collision.name);
        }
    }
    private void Disable_Sword_Collider()
    {
        //gameObject.SetActive(false);
        hit_list.Clear();
        //Destroy(gameObject);
        // Add pool
        SwordWindPool.Instance.AddToPool(gameObject);

    }
}
