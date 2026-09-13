using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed = 20f;
    public Transform target;
    public int pierceCount;
    private Vector2 direction;
    private readonly HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();
    private string layer;

    private float deathTimer = 2f;

    void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    public void Init(Vector2 dir, float dmg, int pierce, string layer)
    {
        direction = dir;
        damage = dmg;
        pierceCount = pierce;
        transform.right = dir;
        this.layer = layer;
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
        enemy.TakeDamage(damage);
    }
}
