using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debuff_Container : MonoBehaviour
{
    [SerializeField] private float space = 3f;
    private Dictionary<string, float> maxDebuffDuration = new Dictionary<string, float>();
    public void UpdateDebuffIcon(Dictionary<string, float> debuffer)
    {
        float x = 0f;
        foreach (KeyValuePair<string, float> debuff in debuffer)
        {
            string debuff_name = debuff.Key;
            float debuff_time = debuff.Value;
            if (!maxDebuffDuration.ContainsKey(debuff_name)) maxDebuffDuration.Add(debuff_name, debuff_time);
            else if (maxDebuffDuration[debuff_name] < debuff_time) maxDebuffDuration[debuff_name] = debuff_time;

            GameObject debuffIcon = transform.Find(debuff_name).gameObject;
            if (debuffIcon != null)
            {
                Image iconImage = debuffIcon.GetComponent<Image>();
                if (debuff_time > 0f)
                {
                    debuffIcon.SetActive(true);
                    RectTransform rectTransform = debuffIcon.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(x, 0f);

                    iconImage.fillAmount = debuff_time / maxDebuffDuration[debuff_name];
                    x += rectTransform.sizeDelta.x + space;
                }
                else debuffIcon.SetActive(false);
            }
        }
    }
}
