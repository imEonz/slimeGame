using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WeaponLevelReward
{
    public enum RewardType { Damage, FireRate, AddInstance, SpriteChange, Homing, Range, Pierce, Count, Clone }
    public RewardType type;
    public float value;
    public Sprite sprite;
}

[System.Serializable]
public class OnLevelUp
{
    public int level;
    public List<WeaponLevelReward> rewards;
}