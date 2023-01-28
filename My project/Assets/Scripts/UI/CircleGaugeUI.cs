using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleGaugeUI : MonoBehaviour
{
    public static CircleGaugeUI Instance { get; private set; }

    [SerializeField] private Image borderImage;
    [SerializeField] private Image gaugeImage;
    float maxGauge = 100f;
    float curGauge;
    [SerializeField] private Color maxColor;
    private void OnEnable()
    {
        Instance = this;
        curGauge = 0f;
    }
    public void updateGauge(float gauge)
    {
        curGauge = gauge;
        gaugeImage.fillAmount = curGauge / maxGauge;
        if (curGauge == maxGauge)
        {
            borderImage.color = maxColor;
        }
        else borderImage.color = new Color(255f, 255f, 255f);
    }
}
