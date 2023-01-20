using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo_Collider : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public Vector2 damageForce;
    public AudioSource[] audioSource;
    private List<string> hit_list;
    int rand;
    private void Awake()
    {
        //animator = GetComponent<Animator>();
        hit_list = new List<string>();
    }
    private void OnEnable()
    {
        rand = Random.Range(0, audioSource.Length);
        if (audioSource[rand] != null) audioSource[rand].PlayOneShot(audioSource[rand].clip);
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
                collision.GetComponent<Enemy>().TakeDamage(damage, damageForce);
            }
            else if (tag == "Boss")
            {
                collision.GetComponent<Boss>().TakeDamage(damage);
            }
            //else Debug.Log("We hit " + collision.name);
        }
    }
    private void OnDisable()
    {
        hit_list.Clear();
    }
    /*private void Disable_Sword_Collider()
    {
        gameObject.SetActive(false);
        hit_list.Clear();
    }*/
}
