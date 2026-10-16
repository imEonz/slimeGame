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
    private readonly HashSet<Skill> activeSkills = new();
    private PlayerLeveler playerLeveler;

    void Start()
    {
        playerLeveler = FindAnyObjectByType<PlayerLeveler>();
    }

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



    public bool AddSkill(SkillReward reward)
    {
        if (reward == null || reward.Skill == null) return false;

        RewardInstance existing = activeRewards.Find(r => r.data == reward);

        if (existing != null)
        {
            existing.experience++;
            if (existing.experience >= existing.curLevel + 1)
            {
                existing.curLevel++;
                existing.experience = 0;
            }
            notificationManager.ShowPickup(existing);
            return true;
        }

        var instance = new RewardInstance(reward);
        activeRewards.Add(instance);
        activeSkills.Add(reward.Skill);

        reward.Skill.Activate();
        notificationManager.ShowPickup(instance);
        return true;
    }

    private void OnDestroy()
    {
        foreach (var s in activeSkills) s.Deactivate();
        activeSkills.Clear();
    }
    public List<RewardInstance> GetRewardInstances()
    {
        return activeRewards;
    }
}
