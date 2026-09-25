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
                foreach(OnLevelUp item in weaponReward.OnLevelUps)
                {
                    if(item.level != existing.curLevel) continue;
                    ApplyLevelReward(existing,item);
                }

            }
            notificationManager.ShowPickup(existing);
        }
        else
        {
            CreateWeapon(weaponReward);
        }
    }


    private void ApplyLevelReward(ActiveWeapon weapon, OnLevelUp levelData)
    {
        WeaponReward weaponData = (WeaponReward)weapon.data;
        if (weaponData == null) return;
        if(levelData == null) return;

        foreach (WeaponLevelReward reward in levelData.rewards)
        {
            switch (reward.type)
            {
                case WeaponLevelReward.RewardType.Damage:
                    foreach(var item in weapon.controllers) item.SetDamage(reward.value);
                    break;

                case WeaponLevelReward.RewardType.FireRate:
                    foreach(var item in weapon.controllers) item.SetFireRate(reward.value);
                   
                    break;

                case WeaponLevelReward.RewardType.AddInstance:
                    for (int i = 0; i < reward.value; i++)
                    {
                        CreateWeapon(weaponData);
                    }
                    break;
                case WeaponLevelReward.RewardType.SpriteChange:
                
                    break;
                case WeaponLevelReward.RewardType.Pierce:
                    foreach(var item in weapon.controllers) item.SetPierce((int)reward.value);
                    break;

                case WeaponLevelReward.RewardType.Count:
                    foreach(var item in weapon.controllers) item.SetShootCount((int)reward.value);
                    break;

                case WeaponLevelReward.RewardType.Range:
                    foreach(var item in weapon.controllers) item.SetRange(reward.value);
                    break;
            }
        }
    }
    
    private OnLevelUp GetLevelUp(ActiveWeapon activeWeapon)
    {
        OnLevelUp highest = null;
        WeaponReward weaponData = (WeaponReward)activeWeapon.data;
        foreach(OnLevelUp item in weaponData.OnLevelUps)
        {
            if(item.level != activeWeapon.curLevel) continue;
            highest = item;
        }
        return highest;
    }

    private void CreateWeapon(WeaponReward weaponData)
    {
        ActiveWeapon existing = activeWeapons.Find(w => w.data == weaponData);
        GameObject go = Instantiate(weaponData.WeaponPrefab, weaponHolder);
        WeaponController newController = go.GetComponent<WeaponController>();

        if(existing != null)
        {
            existing.controllers.Add(newController);

            newController.SetDamage(weaponData.BaseDamage);
            newController.SetFireRate(weaponData.BaseFireRate);

            SyncWeaponToCurLevel(existing, newController);     
            
        }
        else
        {
            ActiveWeapon newWeapon = new ActiveWeapon
            {
                data = weaponData,
                controllers = new List<WeaponController>()
            };

            newWeapon.controllers.Add(newController);
            activeWeapons.Add(newWeapon);        

            newController.SetDamage(weaponData.BaseDamage);  
            newController.SetFireRate(weaponData.BaseFireRate);

            ApplyLevelReward(newWeapon, GetLevelUp(newWeapon));
            
            notificationManager.ShowPickup(newWeapon);
        }
    }

    private void SyncWeaponToCurLevel(ActiveWeapon activeWeapon, WeaponController controller)
    {
        WeaponReward weaponData = (WeaponReward)activeWeapon.data;
        if (weaponData == null) return;

        foreach (OnLevelUp levelUp in weaponData.OnLevelUps)
        {
            if (levelUp.level > activeWeapon.curLevel) continue;

            foreach (WeaponLevelReward reward in levelUp.rewards)
            {
                switch (reward.type)
                {
                    case WeaponLevelReward.RewardType.Damage:
                        controller.SetDamage(reward.value);
                        break;
                    case WeaponLevelReward.RewardType.FireRate:
                        controller.SetFireRate(reward.value);
                        
                        break;
                    case WeaponLevelReward.RewardType.Pierce:
                        controller.SetPierce((int)reward.value);
                        break;
                    case WeaponLevelReward.RewardType.Count:
                        controller.SetShootCount((int)reward.value);
                        break;
                    case WeaponLevelReward.RewardType.Range:
                        controller.SetRange(reward.value);
                        break;
                }
            }
        }
    }
}
