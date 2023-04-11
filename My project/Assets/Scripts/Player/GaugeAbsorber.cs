using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeAbsorber : MonoBehaviour
{
    private Hashashin_Attack hashashin_Attack;

    private List<string> hit_list;

    private void Awake()
    {
        hit_list = new List<string>();
        hashashin_Attack = transform.parent.GetComponent<Hashashin_Attack>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string layer = LayerMask.LayerToName(collision.gameObject.layer);
        string name = collision.name;
        if (!hit_list.Contains(name))
        {
            hit_list.Add(name);

            if ((layer == "Enemy" || layer == "Boss"))
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
