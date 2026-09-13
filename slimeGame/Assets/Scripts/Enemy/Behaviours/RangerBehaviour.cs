
using Unity.VisualScripting;
using UnityEngine;

public class RangerBehaviour : EnemyBehaviour
{
    [SerializeField] 
    private float speed;
    [SerializeField]
    private float range;
    [SerializeField]
    private float rangeOffset;
    public override void Tick(float deltaTime)
    {
        float distance = Vector2.Distance(enemy.transform.position, player.position);
        Vector2 dir = ((Vector2)player.position - (Vector2)enemy.transform.position).normalized;

        if(distance > range)
        {
            
            Vector2 nextPos = (Vector2)enemy.transform.position + dir * speed * deltaTime;
            rb.MovePosition(nextPos);
        }
        else if (distance < range - rangeOffset)
        {
            Vector2 nextPos = (Vector2)enemy.transform.position + -dir * speed * deltaTime;
            rb.MovePosition(nextPos);
        }

    }
}