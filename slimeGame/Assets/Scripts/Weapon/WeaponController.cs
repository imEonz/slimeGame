using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour, IWeaponController
{

    [SerializeField]
    private Transform player;
    private float orbitRadius = 1.3f;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float range;
    [SerializeField]
    private float bulletCooldown;
    private float cooldownTimer;
    [SerializeField]
    private LayerMask layer;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if(player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        cooldownTimer += Time.deltaTime;
        if(cooldownTimer > bulletCooldown)
        {
            if(!GetClosestEnemyInRange()) return;
            cooldownTimer = 0f;
            Shot();
        }
    }

    public void Shot()
    {
        if(!GetClosestEnemyInRange()) return;
        
        Transform target = GetClosestEnemyInRange();
        if(target == null) return;
        
        
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Bullet bullet = proj.GetComponent<Bullet>();
        bullet.target = target;
        bullet.damage = this.damage;
    }

    private Transform GetClosestEnemyInRange()
    {
        Transform closestEnemy = null;
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range, layer);

        float closestDistance = Mathf.Infinity;

        foreach (var enemy in enemiesInRange)
        {
            if(enemy.GetComponent<Enemy>().enemyState != Enemy.State.Alive) continue;
            
            float distance = Vector2.Distance(transform.position, enemy.gameObject.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public void SetFireRate(float firerate)
    {
        this.bulletCooldown = firerate;
    }

    
}
