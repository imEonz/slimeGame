
using UnityEngine;

public class ChaserBehaviour : EnemyBehaviour
{
    [SerializeField] 
    private float speed;
    public override void Tick(float deltaTime)
    {
        Vector2 dir = ((Vector2)player.position - (Vector2)enemy.transform.position).normalized;
        Vector2 nextPos = (Vector2)enemy.transform.position + dir * speed * deltaTime;
        rb.MovePosition(nextPos);
    }
}