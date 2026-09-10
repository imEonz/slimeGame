using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    private float orbitRadius = 1.3f;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float range;
    [SerializeField]
    private float bulletCooldown;
    private float cooldownTimer;
    [SerializeField]
    private LayerMask layer;

    void Start()
    {
        if(player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if(cooldownTimer > bulletCooldown)
        {
            if(!GetClosestEnemyInRange()) return;
            cooldownTimer = 0f;
            Shot();
        }
    }

    public Transform GetClosestEnemyInRange()
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

    public void Shot()
    {
        if(!GetClosestEnemyInRange()) return;
        TeleportAroundPlayer();
        
        Transform target = GetClosestEnemyInRange();
        if(target == null) return;
        
        
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Bullet bullet = proj.GetComponent<Bullet>();
        bullet.target = target;
    }

    public void TeleportAroundPlayer()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * orbitRadius;

        transform.position = (Vector2)player.position + offset;
    }
}
