using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBus : MonoBehaviour
{
    private static readonly Dictionary<TriggerType, List<Action<TriggerContext>>> listeners = new();

    public static event Action<TriggerContext> OnAny;

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

        try { OnAny?.Invoke(ctx); }
        catch (Exception e) { Debug.LogException(e); }

        if (!listeners.TryGetValue(ctx.type, out var list) || list.Count == 0) return;


        var snapshot = list.ToArray();
        for (int i = 0; i < snapshot.Length; i++)
        {
            try { snapshot[i](ctx); }
            catch (Exception e) { Debug.LogException(e); }
        }
    }

    public static void Clear()
    {
        listeners.Clear();
        OnAny = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => Clear();
}
