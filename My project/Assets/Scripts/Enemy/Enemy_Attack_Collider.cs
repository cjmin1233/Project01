using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_Collider : MonoBehaviour
{
    public float damage;
    public AudioSource audioSource;
    private void OnEnable()
    {
        if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "Player")
        {
            collision.GetComponent<Player>().TakeDamage(damage);
        }
    }
    private void PlayAudio()
    {
        if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);
    }

}
