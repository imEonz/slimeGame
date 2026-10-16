using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    private readonly HashSet<IDamagable> alreadyHit = new HashSet<IDamagable>();
    private float damage;
    public void Init(float damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        IDamagable target = collision.GetComponent<IDamagable>();
        if (target == null) return;

        if (!alreadyHit.Add(target)) return;

        target.TakeDamage(damage);

        SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.GetAudioClip("explosion"),SoundManager.Instance.transform,3f);
    }

    public void OnEnd()
    {
        Destroy(this.gameObject);
    }
}
