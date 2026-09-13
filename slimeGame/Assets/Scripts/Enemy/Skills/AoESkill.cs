using UnityEngine;

class AoESkill : EnemySkill
{
    [SerializeField] 
    private float damage = 15f;
    [SerializeField]
    private float radius = 4f;
    [SerializeField] 
    private float windUp = 0.5f;
    [SerializeField] 
    private LayerMask playerLayer;

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
            Fire();
            fired = true;
        }
        return timer < windUp;
    }

    private void Fire()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, radius, playerLayer);
        foreach (var hit in hits)
        {
            PlayerController other = hit.GetComponent<PlayerController>();
            if (other != null && other.GetPlayerState() == PlayerController.State.Moving || other.GetPlayerState() == PlayerController.State.Still) other.TakeDamage(damage);
        }
    }
}