using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/Lifesteal")]
public class LifestealEffect : SkillEffect
{
    [Tooltip("Fraction of damage dealt that is healed. 0.1 = 10%")]
    [Range(0f, 1f)] public float lifestealRatio = 0.1f;

    [Tooltip("Ignore damage and heal a flat % of max HP per hit instead")]
    public bool flatPercentOfMaxHP = false;
    public float flatPercent = 1f;

    private PlayerController player;

    public override void Execute(TriggerContext ctx)
    {
        if (ctx.type != TriggerType.OnHit) return;

        if (player == null) player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        float percent;
        if (flatPercentOfMaxHP)
        {
            percent = flatPercent;
        }
        else
        {
            float healAmount = ctx.damage * lifestealRatio;
            percent = healAmount / player.GetPlayerMaxHP();
        }

        if (percent > 0f) player.HealDamage(percent);
    }
}