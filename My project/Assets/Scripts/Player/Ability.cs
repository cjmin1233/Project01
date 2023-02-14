using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    //public int index;
    //public int Type;
    //public int Tier;
    public int index;
    public bool hidden;
    public int level;
    public int weight;
    //public bool isAppeared = false;
    //public bool isSelected = false;
    public void IncreaseLevel()
    {
        level++;
    }
}
