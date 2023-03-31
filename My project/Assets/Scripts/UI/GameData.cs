using System;

[Serializable]
public class Data
{
    public int weaponType;
    public int sceneNumber;
    public int gold;
    public float curHP;
    public float maxHP;
    public float[] position = new float[3];
    public int[] select_log = new int[1];
}