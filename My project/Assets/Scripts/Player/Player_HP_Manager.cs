using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_HP_Manager : MonoBehaviour
{
    [SerializeField]
    private Slider HP_Bar;
    [SerializeField]
    private Text HP_Text;

    private GameObject Player;
    float MaxHP;
    float CurHP;

    private void OnEnable()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    public void HandleHP()
    {
        MaxHP = Player.GetComponent<Player>().MaxHP;
        CurHP = Player.GetComponent<Player>().CurHP;
        HP_Bar.value = CurHP / MaxHP;
        HP_Text.text = ((int)CurHP).ToString() + " / " + ((int)MaxHP).ToString();
    }
}
