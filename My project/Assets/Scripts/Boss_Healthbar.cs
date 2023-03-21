using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Healthbar : MonoBehaviour
{
    private Slider Slider;
    private Color Low = Color.red;
    private Color High = Color.green;
    public void SetHealth(float currentHealth, float maxHealth)
    {
        if (Slider == null) Slider = GetComponent<Slider>();
        Slider.maxValue = maxHealth;
        Slider.value = currentHealth;
        Debug.Log(Slider.normalizedValue);
        Slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Low, High, Slider.normalizedValue);
    }
}
