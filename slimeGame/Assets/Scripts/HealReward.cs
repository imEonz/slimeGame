using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealReward", menuName = "ConsumeReward/HealReward")]
public class HealReward : ConsumeReward
{
    public override void Apply(PlayerController player, int level)
    {

        player.HealDamage(Random.Range(0f + (0.2f * (level - 1f)),0.2f*level));
    }
}
