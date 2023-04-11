using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hashashin_Enemy_Finder : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private BoxCollider2D player_collider;
    private Vector3 player_scale;
    private Vector3 player_collider_offset;
    private void Awake()
    {
        player_collider = player.GetComponent<BoxCollider2D>();
        player_scale = player.transform.lossyScale;
        player_collider_offset = player_collider.offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;

        if(tag=="Enemy" || tag == "Boss")
        {
            player.transform.position = collision.bounds.center;
            player.transform.Translate(new Vector3(-1f * player_collider_offset.x * player_scale.x, -1f * player_collider_offset.y * player_scale.y, 0));
        }
    }
}
