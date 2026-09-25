using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    PlayerController player;
    [SerializeField]
    NotificationManager notificationManager;
    [SerializeField]
    private List<ActiveWeapon> activeWeapons = new List<ActiveWeapon>();

    [SerializeField]
    private WeaponReward starterWeapon;
    [SerializeField]
    private Transform weaponHolder;

    private float firerateMult = 1f;
    private float dmgMult = 1f;
    public void SetFirerateMult(float mult)
    {
        firerateMult = mult;
        RecalculateAll();
    }
    public void SetDmgMult(float mult)
    {
        dmgMult = mult;
        RecalculateAll();
    }

    void Start()
    {
        PickupWeapon(starterWeapon);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            CreateWeapon(starterWeapon);
        }
    }

    public void PickupWeapon(WeaponReward weaponReward)
    {
        ActiveWeapon existing = activeWeapons.Find(w => w.data == weaponReward);

        if (existing != null)
        {
            existing.experience++;
            if (existing.experience >= existing.curLevel + 1)
            {
                existing.curLevel++;
                existing.experience = 0;

                foreach (OnLevelUp item in weaponReward.OnLevelUps)
                {
                    if (item.level != existing.curLevel) continue;
                    foreach (var reward in item.rewards)
                    {
                        if (reward.type == WeaponLevelReward.RewardType.AddInstance)
                        {
                            for (int i = 0; i < reward.value; i++)
                                CreateWeapon(weaponReward);
                        }
                    }
                }

                RecalculateWeapon(existing);
            }
            notificationManager.ShowPickup(existing);
        }
        else
        {
            CreateWeapon(weaponReward);
        }
    }

    private void CreateWeapon(WeaponReward weaponData)
    {
        ActiveWeapon existing = activeWeapons.Find(w => w.data == weaponData);
        GameObject go = Instantiate(weaponData.WeaponPrefab, weaponHolder);
        WeaponController newController = go.GetComponent<WeaponController>();

        if (existing != null)
        {
            existing.controllers.Add(newController);
            RecalculateWeapon(existing);
        }
        else
        {
            ActiveWeapon newWeapon = new ActiveWeapon
            {
                data = weaponData,
                controllers = new List<WeaponController> { newController }
            };
            activeWeapons.Add(newWeapon);
            RecalculateWeapon(newWeapon);
            notificationManager.ShowPickup(newWeapon);
        }
    }

    private void RecalculateAll()
    {
        foreach (var weapon in activeWeapons) RecalculateWeapon(weapon);
    }

    private void RecalculateWeapon(ActiveWeapon weapon)
    {
        WeaponReward weaponData = (WeaponReward)weapon.data;
        if (weaponData == null) return;

        float rawFireRate = weaponData.BaseFireRate;
        float rawDamage = weaponData.BaseDamage;
        int pierce = 0, count = 0;
        float range = 0;

        foreach (OnLevelUp levelUp in weaponData.OnLevelUps)
        {
            if (levelUp.level > weapon.curLevel) continue;

            foreach (WeaponLevelReward reward in levelUp.rewards)
            {
                switch (reward.type)
                {
                    case WeaponLevelReward.RewardType.FireRate: rawFireRate = reward.value; break;
                    case WeaponLevelReward.RewardType.Damage:   rawDamage = reward.value;   break;
                    case WeaponLevelReward.RewardType.Pierce:   pierce = (int)reward.value; break;
                    case WeaponLevelReward.RewardType.Count:    count = (int)reward.value;  break;
                    case WeaponLevelReward.RewardType.Range:    range = reward.value;       break;
                }
            }
        }

        float finalFireRate = rawFireRate * firerateMult;
        float finalDamage = rawDamage * dmgMult;

        foreach (var controller in weapon.controllers)
        {
            controller.SetFireRate(finalFireRate);
            controller.SetDamage(finalDamage);
            if (pierce > 0) controller.SetPierce(pierce);
            if (count > 0) controller.SetShootCount(count);
            if (range > 0) controller.SetRange(range);
        }
    }
}
