using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/Explosion")]
public class OnDeathExplosion : SkillEffect
{
    [SerializeField] private GameObject explosionPrefab;
    public override void Execute(TriggerContext ctx)
    {
        if (ctx.type != TriggerType.OnKill) return;

        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = ctx.position;
        explosion.GetComponent<ExplosionScript>().Init(ctx.enemy.GetMaxHP()/10);
    }
}