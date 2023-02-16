using System;

[Serializable]
public class Data
{
    public int weaponType;
    public int sceneNumber;
    public float curHP;
    public float maxHP;
    public float[] position = new float[3];
    //public bool[] ability = new bool[10];
    public int[] test_arr = new int[5];
    public int[] select_log = new int[1];
}