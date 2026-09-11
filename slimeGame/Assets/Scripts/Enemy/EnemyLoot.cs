using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyLoot
{
    [SerializeField]
    public ConsumeReward reward;
    [SerializeField,Range(0f,1f)]
    public float dropRate;
}
