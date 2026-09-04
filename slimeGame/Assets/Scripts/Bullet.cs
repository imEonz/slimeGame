using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float speed = 20f;
    public Transform target;
    private float deathTimer;

    void FixedUpdate()
    {
        deathTimer += Time.deltaTime;
        if(deathTimer > 20) Destroy(gameObject);
        MoveTowardsTarget();

    }

    private void MoveTowardsTarget()
    {
        if(target == null) return;
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Enemy") return;
        if (collision.tag != "Map") Destroy(gameObject);

        collision.GetComponent<Enemy>().TakeDamage(damage);

        Destroy(gameObject);
    }
}
