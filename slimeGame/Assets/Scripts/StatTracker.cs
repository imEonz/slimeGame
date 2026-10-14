using System;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : MonoBehaviour
{
    public static StatTracker Instance { get; private set; }

    private readonly Dictionary<TriggerType, int> counts = new();
    private readonly Dictionary<TriggerType, float> damageTotals = new();
    private readonly Dictionary<TriggerType, float> amountTotals = new();

    public event Action<TriggerType> Changed;

    void Awake()
    {
        Instance = this;
        TriggerBus.OnAny += Record;
    }

    void OnDestroy()
    {
        TriggerBus.OnAny -= Record;
        if (Instance == this) Instance = null;
    }

    private void Record(TriggerContext ctx)
    {
        counts[ctx.type] = GetCount(ctx.type) + 1;
        damageTotals[ctx.type] = GetDamage(ctx.type) + ctx.damage;
        amountTotals[ctx.type] = GetAmount(ctx.type) + ctx.amount;
        Changed?.Invoke(ctx.type);
    }

    public int GetCount(TriggerType t)    => counts.TryGetValue(t, out var v) ? v : 0;
    public float GetDamage(TriggerType t) => damageTotals.TryGetValue(t, out var v) ? v : 0f;
    public float GetAmount(TriggerType t) => amountTotals.TryGetValue(t, out var v) ? v : 0f;

    public int Kills => GetCount(TriggerType.OnKill);
    public int TotalPierced => GetCount(TriggerType.OnPierce);
    public int TotalConsumed => GetCount(TriggerType.OnConsume);
    public float TotalDamageDealt => GetDamage(TriggerType.OnHit);
    public float TotalXpGrabbed => GetAmount(TriggerType.OnXpGrabbed);
    public float TotalHpHealed => GetAmount(TriggerType.OnHeal);
    public void ResetAll()
    {
        counts.Clear(); damageTotals.Clear(); amountTotals.Clear();
    }
}
