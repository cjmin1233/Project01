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

    [SerializeField]
    private GameObject Player;
    int MaxHP;
    int CurHP;


    public void HandleHP()
    {
        MaxHP = Player.gameObject.GetComponent<Player>().MaxHP;
        CurHP = Player.gameObject.GetComponent<Player>().CurHP;
        HP_Bar.value = (float)CurHP / (float)MaxHP;
        HP_Text.text = CurHP.ToString() + " / " + MaxHP.ToString();
    }
}
