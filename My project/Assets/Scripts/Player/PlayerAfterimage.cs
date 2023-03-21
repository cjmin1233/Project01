using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterimage : MonoBehaviour
{
    private float alpha;
/*    private float activeTime = .2f;
    private float timeActivated;
    private float alphaSet = 1.0f;
    private float alphaSubtracter = 0.0083f;
*/
    private Transform player;

    private SpriteRenderer sr;
    private SpriteRenderer playerSR;

    private Color color;


    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        sr = GetComponent<SpriteRenderer>();

        //alpha = alphaSet;
        sr.sprite = playerSR.sprite;
        transform.SetPositionAndRotation(player.position, player.rotation);

        //timeActivated = Time.time;
        StartCoroutine(StartAfterImage());
    }
    private IEnumerator StartAfterImage()
    {
        color = new Color(1f, 1f, 1f, 1f);
        for (alpha = 1f; alpha > 0.2f; alpha -= Time.smoothDeltaTime * 2f)
        {
            color.r = alpha;
            color.g = alpha;
            color.b = alpha;
            //color.a = alpha;
            sr.color = color;
            yield return null;
        }
        AfterimagePool.Instance.AddToPool(gameObject);
    }
}
