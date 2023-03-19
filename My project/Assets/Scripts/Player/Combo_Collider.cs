using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo_Collider : MonoBehaviour
{
    public int fxType;
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
        playerAttack = transform.parent.GetComponent<PlayerAttack>();
        player = transform.parent.GetComponent<Player>();
        critical = false;
    }
    private void OnEnable()
    {
        hit_list = new List<string>();
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
                collision.GetComponent<Enemy_Default>().TakeDamage(damage, damageForce, fxType);
                if (playerAttack.sword_cursed_enable) player.Heal(2f);
            }
        }
    }
/*    public void PlayAudio()
    {
        rand = Random.Range(0, audioSource.Length);
        if (audioSource[rand] != null) audioSource[rand].PlayOneShot(audioSource[rand].clip);
    }
    public void OnColliderDisable()
    {
        hit_list.Clear();
    }
*/}
