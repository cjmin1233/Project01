using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Healthbar : MonoBehaviour
{
    private Slider Slider;
    [SerializeField] private Color Low;
    [SerializeField] private Color High;
    private void OnEnable()
    {
        Slider = GetComponent<Slider>();
    }
    public void SetHealth(float currentHealth, float maxHealth)
    {
        Slider.maxValue = maxHealth;
        Slider.value = currentHealth;

        Slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Low, High, Slider.normalizedValue);
    }
}
