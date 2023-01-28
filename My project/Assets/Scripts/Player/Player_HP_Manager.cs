using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_HP_Manager : MonoBehaviour
{
    public static Player_HP_Manager Instance { get; private set; }

    [SerializeField]
    private Slider HP_Bar;
    [SerializeField]
    private Text HP_Text;

    //private GameObject Player;
    float MaxHP;
    float CurHP;

    private void OnEnable()
    {
        Instance = this;
        //Player = GameObject.FindGameObjectWithTag("Player");
    }
    public void HandleHP(float cur, float max)
    {
        MaxHP = max;
        CurHP = cur;
        HP_Bar.value = CurHP / MaxHP;
        HP_Text.text = ((int)CurHP).ToString() + " / " + ((int)MaxHP).ToString();
    }
}
