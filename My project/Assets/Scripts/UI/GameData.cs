using System;

[Serializable]
public class Data
{
    public int weaponType;
    public float curHP;
    public float maxHP;
    public bool[] ability = new bool[10];
}