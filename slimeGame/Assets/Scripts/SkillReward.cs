using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/Skill Reward")]
public class SkillReward : ConsumeReward
{
    [SerializeField] private Skill skill;
    public Skill Skill => skill;
    
    public override void Apply(PlayerController player, int level) { }

    public override string GetDescription()
    {
        return skill != null ? skill.skillName : base.GetDescription();
    }
}