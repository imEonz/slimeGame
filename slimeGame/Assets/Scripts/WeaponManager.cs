using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    PlayerController player;
    [SerializeField]
    NotificationManager notificationManager;
    [SerializeField]
    private List<ActiveWeapon> activeWeapons = new List<ActiveWeapon>();
    private List<GunController> guns = new List<GunController>();
    private List<GunController> swords = new List<GunController>();
    private List<GunController> bows = new List<GunController>();

    public void PickupWeapon(WeaponReward weaponReward)
    {
        ActiveWeapon existing = activeWeapons.Find(w => w.data == weaponReward);
        WeaponReward weaponData = (WeaponReward)existing.data;

        if (existing != null)
        {
            existing.experience++;
            if (existing.experience >= existing.curLevel + 1)
            {
                existing.curLevel++;
                existing.experience = 0;
                foreach(var item in weaponData.OnLevelUps)
                {
                    if(item.level != existing.curLevel) continue;

                }

            }
            notificationManager.ShowPickup(existing);
        }
        else
        {
            GameObject go = Instantiate(weaponReward.WeaponPrefab, player.transform);
            ActiveWeapon newWeapon = new ActiveWeapon
            {
                data = weaponReward,
                controller = go.GetComponent<GunController>()
            };
            activeWeapons.Add(newWeapon);
            notificationManager.ShowPickup(newWeapon);
        }

        
    }


    private void ApplyLevelReward(ActiveWeapon weapon, OnLevelUp levelData)
    {
        WeaponReward weaponData = (WeaponReward)weapon.data;
        if (weaponData == null) return;

        foreach (WeaponLevelReward reward in levelData.rewards)
        {
            switch (reward.type)
            {
                case WeaponLevelReward.RewardType.Damage:

                    break;

                case WeaponLevelReward.RewardType.FireRate:

                    break;

                case WeaponLevelReward.RewardType.AddInstance:
                    
                    break;

                case WeaponLevelReward.RewardType.SpriteChange:
                    
                    break;
            }
        }
    }
    

}
