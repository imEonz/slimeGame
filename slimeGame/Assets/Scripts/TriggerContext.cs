using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    OnConsume, OnDash, OnKill, OnHit, OnPierce,
    OnDeath, OnPlayerHit, OnXpGrabbed, OnLevelUp
}

public class TriggerContext : MonoBehaviour
{
    public TriggerType type;
    public Transform player;
    public Enemy enemy; 
    public Vector2 position;     
    public float damage; 
    public float amount;
    public int depth;
}
