using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed = 20f;
    public Transform target;
    public int pierceCount;
    public float range;
    protected Vector2 direction;
    private readonly HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();
    private string layer;
    
    [SerializeField]
    private float deathTimer = 2f;

    protected virtual void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    public void Init(Vector2 dir, float dmg, int pierce, string layer, Transform target = null)
    {
        direction = dir;
        damage = dmg;
        pierceCount = pierce;
        transform.right = dir;
        this.layer = layer;
        if(target != null) this.target = target;
        Destroy(gameObject, deathTimer);
    }
    private void MoveTowardsTarget()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != layer) return;
        if (collision.tag == "Map") Destroy(gameObject);
        if (hitTargets.Contains(collision)) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null || enemy.enemyState != Enemy.State.Alive) return;

        hitTargets.Add(collision);
        OnHit(enemy);

        pierceCount--;
        if(pierceCount <= 0) Destroy(gameObject);
    }

    protected virtual void OnHit(Enemy enemy)
    {
        ScoreManager.Instance.AddScore("Hit",20);
        if(hitTargets.Count != 1) ScoreManager.Instance.AddScore("Pierce",40);
        enemy.TakeDamage(damage);
    }

    protected Vector2 RecalculateDirection(Transform enemy)
    {
        if(target == null) return direction;
        return ((Vector2)enemy.position - (Vector2)transform.position).normalized;
    }

    protected Transform GetClosestEnemyInRange()
    {
        Transform closestEnemy = null;
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(this.transform.position, range, LayerMask.GetMask("Enemy"));

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
}
