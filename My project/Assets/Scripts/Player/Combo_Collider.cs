using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo_Collider : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public Vector2 damageForce;
    [HideInInspector] public bool critical;
    private PlayerAttack playerAttack;
    private Player player;

    public AudioSource[] audioSource;
    private List<string> hit_list;
    int rand;
    private void Awake()
    {
        //animator = GetComponent<Animator>();
        hit_list = new List<string>();
        playerAttack = transform.parent.GetComponent<PlayerAttack>();
        player = transform.parent.GetComponent<Player>();
        critical = false;
    }
    private void OnEnable()
    {
        rand = Random.Range(0, audioSource.Length);
        if (audioSource[rand] != null) audioSource[rand].PlayOneShot(audioSource[rand].clip);
        //if (critical) Debug.Log("Critical!");
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
                //collision.GetComponent<Enemy>().TakeDamage(damage, damageForce);
                collision.GetComponent<Enemy_Default>().TakeDamage(damage, damageForce);
            }
            else if (tag == "Boss")
            {
                //collision.GetComponent<Boss>().TakeDamage(damage);
                collision.GetComponent<Enemy_Default>().TakeDamage(damage, damageForce);
            }
            if ((tag == "Enemy" || tag == "Boss") && playerAttack.sword_cursed_enable)
            {
                player.Heal(2f);
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
