using System;
using UnityEngine;
[Serializable]
public struct EnemySummonData
{
    public int enemy_type;
    public Transform enemy_transform;
    public float maxHP;
    public int drop_gold;
    public EnemySummonData(int type, Transform trans, float hp, int gold)
    {
        this.enemy_type = type;
        this.enemy_transform = trans;
        this.maxHP = hp;
        this.drop_gold = gold;
    }
}
