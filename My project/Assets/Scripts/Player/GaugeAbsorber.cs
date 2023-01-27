using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeAbsorber : MonoBehaviour
{
    private Hashashin_Attack hashashin_Attack;
    //private Player player;

    private List<string> hit_list;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        hit_list = new List<string>();
        hashashin_Attack = transform.parent.GetComponent<Hashashin_Attack>();
        //player = transform.parent.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;
        if (!hit_list.Contains(name))
        {
            hit_list.Add(name);

            if ((tag == "Enemy" || tag == "Boss"))
            {
                // 적 타격시 게이지 수급
                hashashin_Attack.GetGauge(20f);
            }
        }
    }
    private void OnDisable()
    {
        hit_list.Clear();
    }
}
