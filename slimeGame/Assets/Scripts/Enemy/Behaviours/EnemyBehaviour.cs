using UnityEngine;

public abstract class EnemyBehaviour : MonoBehaviour
{
    protected Enemy enemy;
    protected Rigidbody2D rb;
    protected Transform player;

    public virtual void Initialize(Enemy enemy)
    {
        this.enemy = enemy;
        this.rb = enemy.rb;
        this.player = enemy.player;
    }

    public abstract void Tick(float deltaTime);
}