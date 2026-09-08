using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private List<StatBuffReward> statBuffRewards = new List<StatBuffReward>();

    public void InstantiateStatBuff(PlayerController player, StatBuffReward statBuff)
    {
        statBuffRewards.Add(statBuff);

        statBuff.Apply(player, statBuff.curLevel);
    }
}
