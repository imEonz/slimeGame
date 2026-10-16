using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponReward", menuName = "ConsumeReward/WeaponReward")]
public class WeaponReward : ConsumeReward
{
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private float baseDamage;
    [SerializeField] private float baseFireRate;
    [SerializeField] private List<OnLevelUp> onLevelUps;
    public GameObject WeaponPrefab => weaponPrefab;
    public List<OnLevelUp> OnLevelUps => onLevelUps;
    public float BaseDamage => baseDamage;
    public float BaseFireRate => baseFireRate;

    public override void Apply(PlayerController player, int level)
    {
        
    }
}