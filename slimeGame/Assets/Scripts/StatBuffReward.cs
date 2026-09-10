using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType {MaxHP, DMG, Speed, Dash, DashCooldown, Firerate}

[CreateAssetMenu(fileName = "StatBuffReward", menuName = "ConsumeReward/StatBuffReward")]
public class StatBuffReward : ConsumeReward
{
    [SerializeField]
    private StatType statType;
    public override void Apply(PlayerController player, int level)
    {
        player.ApplyStatBuff(statType, level);
    }
}
