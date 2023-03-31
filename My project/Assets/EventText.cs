using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventText : MonoBehaviour
{
    [HideInInspector] public string text;
    [SerializeField] private float y_speed;
    private TextMeshProUGUI event_text;
    private float alpha;
    private Color textColor;
    private void Awake()
    {
        event_text = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        event_text.text = text;
        alpha = 1f;
    }
    private void Update()
    {
        transform.Translate(new Vector3(0f, y_speed * Time.deltaTime, 0f));
        alpha -= Time.deltaTime;
        textColor = event_text.color;
        textColor.a = alpha;
        event_text.color = textColor;
        if (alpha <= 0f) gameObject.SetActive(false);
    }
}
