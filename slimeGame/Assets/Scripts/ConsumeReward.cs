using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumeReward : ScriptableObject
{
    [SerializeField] private string rewardName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int maxLevel = 5;
    [SerializeField] public int curLevel;
    [SerializeField, TextArea(3, 10)]
    private string bio;

    public string RewardName => rewardName;
    public Sprite Icon => icon;
    public int MaxLevel => maxLevel;
    public string Bio => bio;

    public abstract void Apply(PlayerController player, int level);

    public virtual string GetDescription()
    {
        return $"{rewardName} LVL: {curLevel}";
    }
}
