using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private List<RewardInstance> activeRewards = new List<RewardInstance>();
    [SerializeField]
    NotificationManager notificationManager;

    public void InstantiateStatBuff(PlayerController player, StatBuffReward statBuff)
    {
        RewardInstance existing = activeRewards.Find(r => r.data == statBuff);

        if (existing != null)
        {
            existing.experience++;

            if (existing.experience >= existing.curLevel + 1 && existing.curLevel < statBuff.MaxLevel)
            {
                existing.curLevel++;
                existing.experience = 0;
                statBuff.Apply(player, existing.curLevel);
            }
            notificationManager.ShowPickup(existing);
        }
        else
        {
            RewardInstance newInstance = new RewardInstance(statBuff);
            activeRewards.Add(newInstance);
            statBuff.Apply(player, newInstance.curLevel);
            notificationManager.ShowPickup(newInstance);
        }

    }
}
