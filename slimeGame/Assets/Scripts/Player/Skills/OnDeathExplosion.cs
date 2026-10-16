using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/Explosion")]
public class OnDeathExplosion : SkillEffect
{
    [SerializeField] private GameObject explosionPrefab;
    public override void Execute(TriggerContext ctx)
    {
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = ctx.position;
        if(ctx.enemy != null) 
        {
            explosion.GetComponent<ExplosionScript>().Init(ctx.enemy.GetMaxHP()/10);
            return;
        }
        else
        {
            explosion.GetComponent<ExplosionScript>().Init(50);
        }
    }
}