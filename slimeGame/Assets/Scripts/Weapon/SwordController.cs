using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SwordController : WeaponController
{
    [SerializeField] 
    private Transform pivot;
    [SerializeField] 
    private float slashArc = 90f;
    [SerializeField] 
    private float slashDuration = 0.2f;
    [SerializeField]
    SpriteRenderer sprite;

    protected override void Update()
    {
        Vector2 orbitDir = GetOrbitDirection();
        if (orbitDir != Vector2.zero) PositionInOrbitTowards(orbitDir);

        base.Update();
    }
    protected override IEnumerator ActivationRoutine()
    {
        sprite.gameObject.SetActive(true);
        yield return base.ActivationRoutine();
        sprite.gameObject.SetActive(false);
    }

    protected override IEnumerator PerformAttack(Vector2 dir, Transform target, Vector2? spawnpos = null)
    {
        if (pierceCount > 0 && projectilePrefab != null)
        {
            SpawnSlashProjectile(dir);
        }

        enemyHit.Clear();

        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - (slashArc / 2f) - 90f;
        float endAngle = baseAngle + (slashArc / 2f) - 90f;

        float t = 0f;
        pivot.rotation = Quaternion.Euler(0, 0, startAngle);

        SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.GetAudioClip("swing"),this.transform,0.1f);

        while (t < slashDuration)
        {
            t += Time.deltaTime;
            float angle = Mathf.Lerp(startAngle, endAngle, t / slashDuration);
            pivot.rotation = Quaternion.Euler(0, 0, angle);
            CheckHitbox();
            yield return null;
        }
    }

    private void CheckHitbox()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            weaponCollider.bounds.center,
            weaponCollider.bounds.size,
            pivot.eulerAngles.z,
            layer);

        foreach (var hit in hits)
        {
            if (enemyHit.Contains(hit)) continue;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null || enemy.enemyState != Enemy.State.Alive) continue;

            enemyHit.Add(hit);
            ScoreManager.Instance.AddScore("Hit",20);
            enemy.TakeDamage(damage);
        }
    }

    private void SpawnSlashProjectile(Vector2 direction)
    {
        GameObject proj = Instantiate(projectilePrefab, pivot.position, quaternion.identity);
        SlashProjectile sp = proj.GetComponent<SlashProjectile>();
        sp.Init(direction, damage, pierceCount, "Enemy");
    }
}
