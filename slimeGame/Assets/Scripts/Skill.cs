using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public TriggerBinding[] bindings;
    public virtual void OnAcquire(PlayerLeveler leveler) { }

    private Dictionary<TriggerBinding, Action<TriggerContext>> subscribed;

    public void Activate()
    {
        subscribed = new();
        foreach (var b in bindings)
        {
            var binding = b;
            Action<TriggerContext> cb = ctx =>
            {
                if (UnityEngine.Random.value > binding.chance) return;
                foreach (var e in binding.effects) e.Execute(ctx);
            };
            subscribed[binding] = cb;
            TriggerBus.Subscribe(binding.trigger, cb);
        }
    }

    public void Deactivate()
    {
        if (subscribed == null) return;
        foreach (var kv in subscribed) TriggerBus.Unsubscribe(kv.Key.trigger, kv.Value);
        subscribed = null;
    }
}