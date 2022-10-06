using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterimage : MonoBehaviour
{
    private float activeTime = .2f;
    private float timeActivated;
    private float alpha;
    private float alphaSet = 1.0f;
    private float alphaSubtracter = 0.0083f;

    private Transform player;

    private SpriteRenderer sr;
    private SpriteRenderer playerSR;

    private Color color;


    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        //alpha = playerSR.color.a;
        //Debug.Log(alpha);
        sr.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;

        timeActivated = Time.time;
    }
    private void Update()
    {
        alpha -= alphaSubtracter;
        color = new Color(1f, 1f, 1f, alpha);
        sr.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            // Add pool
            AfterimagePool.Instance.AddToPool(gameObject);
        }
    }

}
