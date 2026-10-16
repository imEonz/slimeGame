using UnityEngine;

public abstract class EnemySkill : MonoBehaviour
{
    protected Enemy enemy;
    protected Transform player;
    protected float timer;
    protected bool fired;
    [SerializeField] 
    private bool blocksMovement = false;
    public bool BlocksMovement => blocksMovement;

    public virtual void Initialize(Enemy enemy)
    {
        this.enemy = enemy;
        this.player = enemy.player;
    }

    public virtual void Begin()
    {
        
    }

    public abstract bool Tick(float deltaTime);
}
