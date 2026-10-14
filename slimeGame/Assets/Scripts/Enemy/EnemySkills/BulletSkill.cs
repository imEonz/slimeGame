using UnityEngine;

class BulletSkill : EnemySkill
{
    [SerializeField] 
    private GameObject projectilePrefab;
    [SerializeField] 
    private float damage = 5f;
    [SerializeField] 
    private float windUp = 0.5f;

    public override void Begin()
    {
        timer = 0f;
        fired = false;
        Debug.Log("Begin");
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
        Debug.Log("Fire");
        Vector2 dir = ((Vector2)player.position - (Vector2)enemy.transform.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(dir, damage, 1, "Player");
    }
}