using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    OnConsume, OnDash, OnKill, OnHit, OnPierce,
    OnDeath, OnPlayerHit, OnXpGrabbed, OnLevelUp, OnHeal, OnMissConsume
}

public class TriggerContext
{
    public TriggerType type;
    public Transform player;
    public Enemy enemy; 
    public Vector2 position;     
    public float damage; 
    public float amount;
    public int depth;

    public TriggerContext(TriggerType type, Vector2 position, Enemy enemy = null,
    float damage = 0f, float amount = 0f, int depth = 0)
    {
        this.type = type;
        this.position = position;
        this.enemy = enemy;
        this.damage = damage;
        this.amount = amount;
        this.depth = depth;
        this.player = null;
    }
}
