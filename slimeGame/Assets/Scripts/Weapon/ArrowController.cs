using System.Collections;
using UnityEngine;

public class ArrowController : WeaponController
{
    [SerializeField]
    private float spreadAngle = 10f;
    [SerializeField]
    private float spacing = 0.5f;
    private int timesShot;
    protected override void Update()
    {
        Vector2 orbitDir = GetOrbitDirection();
        if (orbitDir != Vector2.zero) PositionInOrbitTowards(orbitDir);

        base.Update();
    }

    protected override IEnumerator ActivationRoutine()
    {
        Vector2 baseDir = GetAttackDirection(out Transform target);
        if (baseDir == Vector2.zero) yield break;
        Vector2 perpendicular = new Vector2(-baseDir.y, baseDir.x);
        timesShot = 0;

        for (int i = 0; i < shootCount; i++)
        {
            float centering = i - (shootCount - 1) / 2f;
            Vector2 spawnPos = (Vector2)transform.position + perpendicular * (centering * spacing);

            float offset = (i - (shootCount - 1) / 2f) * spreadAngle;
            Vector2 dir = Rotate(baseDir, offset);
            timesShot++;
            yield return PerformAttack(dir, target, spawnPos);
        }
    }

    protected override IEnumerator PerformAttack(Vector2 dir, Transform target, Vector2? spawnPos = null)
    {
        GameObject proj = null;
        if(spawnPos == null) proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        else proj = Instantiate(projectilePrefab, (Vector2)spawnPos, Quaternion.identity);
        Arrow projectile = proj.GetComponent<Arrow>();
        projectile.windUp = 0.3f + (0.1f * timesShot);
        projectile.Init(dir, damage, pierceCount, "Enemy", target);
        yield break;
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}
