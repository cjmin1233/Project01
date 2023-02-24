using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Healthbar : MonoBehaviour
{
    public Slider Slider;
    public Color Low;
    public Color High;

    public void SetHealth(float currentHealth, float maxHealth)
    {
        Slider.maxValue = maxHealth;
        Slider.value = currentHealth;

        Slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Low, High, Slider.normalizedValue);
    }
}
