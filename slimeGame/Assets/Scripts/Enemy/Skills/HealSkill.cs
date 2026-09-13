using UnityEngine;

class HealSkill : EnemySkill
{
    [SerializeField] 
    private float healing = 15f;
    [SerializeField]
    private float healRadius = 4f;
    [SerializeField] 
    private float windUp = 0.5f;
    [SerializeField] 
    private LayerMask enemyLayer;

    public override void Begin()
    {
        timer = 0f;
        fired = false;
    }
    public override bool Tick(float deltaTime)
    {
        timer += deltaTime;
        if (!fired && timer >= windUp)
        {
            Heal();
            fired = true;
        }
        return timer < windUp;
    }

    private void Heal()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, healRadius, enemyLayer);
        foreach (var hit in hits)
        {
            Enemy other = hit.GetComponent<Enemy>();
            if (other != null && other.enemyState == Enemy.State.Alive) other.Heal(healing);
        }
    }
}