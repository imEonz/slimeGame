using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour, IWeaponController
{

    [SerializeField]
    protected  Transform player;
    [SerializeField]
    protected GameObject projectilePrefab;
    [SerializeField]
    protected float damage;
    [SerializeField]
    protected float range;
    [SerializeField]
    protected float bulletCooldown;
    protected float cooldownTimer;
    [SerializeField]
    protected int pierceCount;
    [SerializeField]
    protected int shootCount;
    [SerializeField]
    protected float shootdelay;
    [SerializeField]
    protected float orbitRadius = 1.3f;
    [SerializeField] 
    protected float orbitOffsetDegrees = 0f;
    protected Coroutine processCoroutine;
    [SerializeField]
    protected LayerMask layer;

    protected Collider2D weaponCollider;

    protected readonly List<Collider2D> enemyHit = new List<Collider2D>();
    protected PlayerController playerController;

    protected virtual void Start()
    {
        if(player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if(playerController == null) playerController = player.GetComponent<PlayerController>();
        weaponCollider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        cooldownTimer += Time.deltaTime;
        if(cooldownTimer > bulletCooldown)
        {
            if (!CanFire()) return;
            cooldownTimer = 0;
            Shot();
        }
    }

    protected virtual void Shot()
    {
        if (!CanFire()) return;
        if (processCoroutine != null) return;
        processCoroutine = StartCoroutine(RunActivation());
    }

    private IEnumerator RunActivation()
    {
        yield return ActivationRoutine();
        processCoroutine = null;
    }

    protected virtual IEnumerator ActivationRoutine()
    {
        
        for (int i = 0; i < shootCount; i++)
        {
            Vector2 dir = GetAttackDirection(out Transform target);
            if (dir != Vector2.zero) yield return PerformAttack(dir, target);
            if (i < shootCount - 1) yield return new WaitForSeconds(shootdelay);
        }
    }

    protected virtual IEnumerator PerformAttack(Vector2 dir, Transform target)
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Init(dir, damage, pierceCount);
        yield break;
    }

    protected bool TryGetAimPoint(out Vector2 point, out Transform target)
    {
        if (playerController != null && playerController.AutoAim)
        {
            target = GetClosestEnemyInRange();
            if (target == null) { point = Vector2.zero; return false; }
            point = target.position;
            return true;
        }

        target = null;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        point = mouseWorld;
        return true;
    }
    protected virtual bool CanFire() => TryGetAimPoint(out _, out _);
    
    protected Vector2 GetAttackDirection(out Transform target)
    {
        if (!TryGetAimPoint(out Vector2 point, out target)) return Vector2.zero;
        return (point - (Vector2)transform.position).normalized;
    }
    protected Vector2 GetOrbitDirection()
    {
        if (!TryGetAimPoint(out Vector2 point, out _)) return Vector2.zero;
        return (point - (Vector2)player.position).normalized;
    }

    protected void PositionInOrbitAtAngle(float angleDegrees)
    {
        float angle = (angleDegrees + orbitOffsetDegrees) * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * orbitRadius;
        transform.position = (Vector2)player.position + offset;
    }

    protected void PositionInOrbitTowards(Vector2 direction)
    {
        if (direction == Vector2.zero) return;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        PositionInOrbitAtAngle(angle);
    }

    protected Transform GetClosestEnemyInRange()
    {
        Transform closestEnemy = null;
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(player.position, range, layer);

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
    public void SetPierce(int pierce)
    {
        this.pierceCount = pierce;
    }
    public void SetShootCount(int count)
    {
        this.shootCount = count;
    }
    public void SetRange(float range)
    {
        this.range = range;
    }

    
}
