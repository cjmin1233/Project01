using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
public class TestAbility : ScriptableObject
{
    public string a_Name;
    public string a_Description;
    public Sprite artwork;

    public int Type;
    public int Tier;
    public int Level;
    //public int weight;

    //public bool isAppeared = false;
    //public bool isSelected = false;
}
