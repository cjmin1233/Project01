using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DamageText : MonoBehaviour
{
    [SerializeField] private float x_speed;
    [SerializeField] private float y_speed;
    [SerializeField] private float ver_ac;
    [SerializeField] private float alphaSpeed;
    private float hor_speed;
    private float ver_speed;
    private TextMeshPro text;
    private float alpha;
    
    [HideInInspector] public float damage;
    [HideInInspector] public float x_dir;
    [HideInInspector] public Color textColor;
    //private Color startColor;
    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }
    private void OnEnable()
    {
        text.text = damage.ToString();
        alpha = 1f;
        ver_speed = Random.Range(0.5f * y_speed, y_speed);
        text.color = textColor;
    }

    private void Update()
    {
        hor_speed = Random.Range(0.5f * x_speed, x_speed);
        ver_speed -= ver_ac * Time.deltaTime;
        transform.Translate(new Vector3(hor_speed * x_dir * Time.deltaTime, ver_speed * Time.deltaTime, 0));
        alpha -= Time.deltaTime;
        textColor = text.color;
        textColor.a = alpha;
        text.color = textColor;
        if (alpha <= 0f) Disable_Object();
    }
    private void Disable_Object()
    {
        DamageTextPool.Instance.AddToPool(gameObject);
    }
}
