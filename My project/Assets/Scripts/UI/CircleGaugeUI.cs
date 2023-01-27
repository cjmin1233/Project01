using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleGaugeUI : MonoBehaviour
{
    [SerializeField] private Image borderImage;
    [SerializeField] private Image gaugeImage;
    float maxGauge = 100f;
    public float curGauge;
    [SerializeField] private Color maxColor;
    private void Start()
    {
        curGauge = 0f;
    }
    private void Update()
    {
        gaugeImage.fillAmount = curGauge / maxGauge;
        if (curGauge == maxGauge)
        {
            borderImage.color = maxColor;
        }
        else borderImage.color = new Color(255f, 255f, 255f);
    }
}
