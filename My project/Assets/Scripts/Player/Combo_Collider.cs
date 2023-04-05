using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo_Collider : MonoBehaviour
{
    public int fxType;
    [HideInInspector] public float damage;
    [HideInInspector] public Vector2 damageForce;
    [HideInInspector] public bool critical = false;
    [HideInInspector] public bool absorb_enable = false;
    private PlayerAttack playerAttack;
    private Player player;

    public AudioSource[] audioSource;
    private List<string> hit_list = new List<string>();
    int rand;
    private void Awake()
    {
        //animator = GetComponent<Animator>();
        playerAttack = transform.parent.GetComponent<PlayerAttack>();
        player = transform.parent.GetComponent<Player>();
    }
    private void OnEnable()
    {
        rand = Random.Range(0, audioSource.Length);
        if (audioSource[rand] != null) audioSource[rand].PlayOneShot(audioSource[rand].clip);
    }
    private void OnDisable()
    {
        hit_list.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;
        if (!hit_list.Contains(name))
        {
            hit_list.Add(name);
            
            if(tag=="Enemy" || tag == "Boss")
            {
                if (critical) collision.GetComponent<Enemy_Default>().TakeDamage(damage, damageForce, critical, Color.yellow, 5);
                else collision.GetComponent<Enemy_Default>().TakeDamage(damage, damageForce, critical, Color.white, fxType);
                if (absorb_enable) player.Heal(2f);
            }
        }
    }
}
