using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ability : MonoBehaviour
{
    public int index;
    public bool hidden;
    public int level;
    public int weight;

    private GameObject upgradeLevel;

    public void EnableUpgradeLevelText(string text)
    {
        if (upgradeLevel==null) upgradeLevel = transform.Find("UpgradeLevelText")?.gameObject;
        if (upgradeLevel != null) upgradeLevel.GetComponent<TextMeshProUGUI>().text = text;
    }
}
