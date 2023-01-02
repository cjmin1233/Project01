using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Gold_Manager : MonoBehaviour
{
    //[SerializeField] private GameObject Player;
    [SerializeField] private TextMeshProUGUI Text;

    int current_gold;
    
    public void HandleGold(int gold)
    {
        Text.text = gold.ToString();
        //current_gold=Player
        /*MaxHP = Player.gameObject.GetComponent<Player>().MaxHP;
        CurHP = Player.gameObject.GetComponent<Player>().CurHP;
        HP_Bar.value = CurHP / MaxHP;
        HP_Text.text = ((int)CurHP).ToString() + " / " + ((int)MaxHP).ToString();*/
    }
}
