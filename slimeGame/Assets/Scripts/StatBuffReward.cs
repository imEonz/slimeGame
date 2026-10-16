using UnityEngine;

public enum StatType {MaxHP, DMG, Speed, Dash, DashCooldown, Firerate}

[CreateAssetMenu(fileName = "StatBuffReward", menuName = "ConsumeReward/StatBuffReward")]
public class StatBuffReward : ConsumeReward
{
    [SerializeField]
    private StatType statType;
    [SerializeField]
    private float[] scaling;
    public override void Apply(PlayerController player, int level)
    {
        player.ApplyStatBuff(this);
    }
    public float GetValue(int level)
    {
        if (scaling == null || scaling.Length == 0 || level <= 0) return 0f;
        return scaling[Mathf.Min(level, scaling.Length) - 1];
    }
    public StatType GetStatType()
    {
        return statType;
    }
}
