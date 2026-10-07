using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBus : MonoBehaviour
{
    private static readonly Dictionary<TriggerType, List<Action<TriggerContext>>> listeners = new();

    public static void Subscribe(TriggerType type, Action<TriggerContext> cb)
    {
        if (!listeners.TryGetValue(type, out var list)) listeners[type] = list = new List<Action<TriggerContext>>();
        list.Add(cb);
    }

    public static void Unsubscribe(TriggerType type, Action<TriggerContext> cb)
    {
        if (listeners.TryGetValue(type, out var list)) list.Remove(cb);
    }

    public static void Fire(TriggerContext ctx)
    {
        if (ctx.depth > 2) return;
        if (!listeners.TryGetValue(ctx.type, out var list)) return;

        for (int i = list.Count - 1; i >= 0; i--) list[i](ctx);
    }

    public static void Clear() => listeners.Clear();
}
