using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeveler : MonoBehaviour
{    
    public class LevelReward
    {
        public string nameReward;
        public enum RewardType {DMG, HP, SPD, FIRERATE}
        public RewardType rewardType;
        public float amount;
        public bool applied;
    }

    private PlayerController player;
    private WeaponManager weaponManager;
    private EnemyManager enemyManager;
    private UIManager uIManager;
    private List<LevelReward> levelRewards = new List<LevelReward>();
    private int level;
    private float curXP;
    private float neededXP;


    [SerializeField]
    private float dmgMult = 1f;
    [SerializeField]
    private float hpMult = 1f;
    [SerializeField]
    private float spdMult = 1f;
    [SerializeField]
    private float firerateMult = 1f;

    void Start()
    {
        player = GameObject.FindAnyObjectByType<PlayerController>();
        weaponManager = GameObject.FindAnyObjectByType<WeaponManager>();
        uIManager = FindAnyObjectByType<UIManager>();
        enemyManager = FindAnyObjectByType<EnemyManager>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            LevelReward test = new LevelReward();
            test.amount = -0.5f;
            test.rewardType = LevelReward.RewardType.FIRERATE;

            ApplyLevelReward(test);
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            LevelReward test = new LevelReward();
            test.amount = 0.5f;
            test.rewardType = LevelReward.RewardType.DMG;

            ApplyLevelReward(test);
        }
    }

    public void ApplyLevelReward(LevelReward levelReward)
    {
        switch (levelReward.rewardType)
        {
            case LevelReward.RewardType.DMG:
            dmgMult += levelReward.amount;
            weaponManager.SetDmgMult(this.dmgMult);
            break;
            case LevelReward.RewardType.HP:
            hpMult += levelReward.amount;
            break;
            case LevelReward.RewardType.SPD:
            spdMult += levelReward.amount;
            break;
            case LevelReward.RewardType.FIRERATE:
            firerateMult += levelReward.amount;
            if(firerateMult < 0.5f) 
            {
                firerateMult = 0.5f;
            }
            weaponManager.SetFirerateMult(firerateMult);
            break;
        }
        levelReward.applied = true;
        levelRewards.Add(levelReward);
    }



    public float GetDMGMult()
    {
        return dmgMult;
    }
    public float GetHPMult()
    {
        return hpMult;
    }
    public float GetSPDMult()
    {
        return spdMult;
    }
    public float GetFirerateMult()
    {
        return firerateMult;
    }

}
