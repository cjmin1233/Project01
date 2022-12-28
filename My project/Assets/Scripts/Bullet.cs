using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 25f;
    public float damage = 40f;
    public GameObject ImpactEffect;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        Invoke("destroyBullet", 2f);
    }
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        string tag = hitInfo.tag;
        if (tag == "Enemy")
        {
            hitInfo.GetComponent<Enemy>().TakeDamage(damage, new Vector2(0,0));
        }
        else if (tag == "Boss")
        {
            Debug.Log("We hit the " + hitInfo.name);
            hitInfo.GetComponent<Boss>().TakeDamage(damage);
        }
        else
        {
            Debug.Log("We hit nothing");
        }
        /*
        /*
        Enemy enemy = hitInfo.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
         */

        Instantiate(ImpactEffect, transform.position, transform.rotation);
        //Destroy(gameObject);
        destroyBullet();
    }
    private void destroyBullet()
    {
        Destroy(gameObject);
    }
}
