using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw_Collider : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int fxType;
    private List<string> hit_list;
    private GameObject playerObject;
    private void Awake()
    {
        hit_list = new List<string>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        transform.localRotation = playerObject.transform.rotation;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;
        if (!hit_list.Contains(name))
        {
            hit_list.Add(name);
            if (tag == "Enemy" || tag == "Boss") collision.GetComponent<Enemy_Default>().TakeDamage(damage, Vector2.zero, false, Color.blue, fxType);
        }
    }
    private void PlayAudioSource()
    {
        if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);
    }
    private void ClearHitList()
    {
        hit_list.Clear();
        // 플레이어 위치로 이동
        transform.position = playerObject.GetComponent<BoxCollider2D>().bounds.center;
    }
}
