using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Healthbar : MonoBehaviour
{
    private Slider Slider;
    public Color Low;
    public Color High;
    //public Vector3 Offset;
    //[HideInInspector] public Transform enemyTransform;

    private void OnEnable()
    {
        Slider = GetComponent<Slider>();
    }
    public void SetHealth(float currentHealth, float maxHealth)
    {
        Slider.gameObject.SetActive(currentHealth < maxHealth);
        Slider.maxValue = maxHealth;
        Slider.value = currentHealth;

        Slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Low, High, Slider.normalizedValue);
    }
}
