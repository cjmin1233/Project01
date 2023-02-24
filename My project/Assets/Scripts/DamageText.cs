using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DamageText : MonoBehaviour
{
    public float floatingSpeed;
    public float alphaSpeed;
    //Text text_tmp;
    TextMeshPro text;
    Color alpha;
    public float damage;

    private Color startColor;
    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        startColor = new Color(255f, 255f, 255f, 255f);
    }
    private void OnEnable()
    {        
        text.text = damage.ToString();
        alpha = startColor;
        Invoke("Disable_Object", 1f);
    }

    private void Update()
    {
        transform.Translate(new Vector3(0, floatingSpeed * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }
    private void Disable_Object()
    {
        //Destroy(gameObject);
        DamageTextPool.Instance.AddToPool(gameObject);
    }
}
