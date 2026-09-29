using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLeveler : MonoBehaviour
{    
    public class LevelReward
    {
        public string nameReward;
        public enum RewardType {DMG, HP, SPD, FIRERATE}
        public enum RewardRarity {COMMON, RARE, LEGENDARY, SECRET}
        public RewardType rewardType;
        public RewardRarity rewardRarity;
        public float amount;
        public bool applied;
        public string desc;
    }

    [System.Serializable]
    public class RewardScaling
    {
        public LevelReward.RewardType type;
        public Vector2[] amountRange = new Vector2[4];
    }

    [SerializeField]
    private RewardScaling[] rewardScaling;

    private PlayerController player;
    private WeaponManager weaponManager;
    private EnemyManager enemyManager;
    private UIManager uIManager;
    private List<LevelReward> levelRewards = new List<LevelReward>();
    private int level = 1;
    private float curXP;
    private float neededXP;
    private bool IsLeveling;

    [SerializeField]
    private GameObject cardObject;
    [SerializeField]
    private Transform cardHolder;
    private int cardAmount = 3;


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

        if(Input.GetKeyDown(KeyCode.L))
        {
            OnLevelUp();
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
            if(firerateMult < 0.1f) 
            {
                firerateMult = 0.1f;
            }
            weaponManager.SetFirerateMult(firerateMult);
            break;
        }
        levelReward.applied = true;
        levelRewards.Add(levelReward);
        player.RecalculateStats();
    }

    private void InitializeCards()
    {
        foreach (Transform child in cardHolder)
        {
            Destroy(child.gameObject);
        }
        var rarities = (LevelReward.RewardRarity[])System.Enum.GetValues(typeof(LevelReward.RewardRarity));
        var types = (LevelReward.RewardType[])System.Enum.GetValues(typeof(LevelReward.RewardType));

        for (int i = 0; i < cardAmount; i++)
        {
            GameObject card = Instantiate(cardObject, cardHolder);
            LevelReward random = new LevelReward();
            random.rewardRarity = rarities[Random.Range(0, rarities.Length)];
            random.rewardType = types[Random.Range(0, types.Length)];
            random.amount = RollAmount(random.rewardType, random.rewardRarity);
            InitializeStrings(random);
            InitializeCardGO(card, random);


            card.GetComponent<Button>().onClick.AddListener(() =>
            {
                ApplyLevelReward(random);
                foreach (Transform child in cardHolder)
                {
                    Destroy(child.gameObject);
                }
                uIManager.ActivateLevel();
                IsLeveling = false;
            });

        }
    }

    private void InitializeCardGO(GameObject card, LevelReward levelReward)
    {
        Image rarityBorder = card.transform.Find("RarityBorder").GetComponent<Image>();
        TMP_Text itemName = card.transform.Find("Name").GetComponent<TMP_Text>();
        TMP_Text itemDesc = card.transform.Find("Desc").GetComponent<TMP_Text>();
        TMP_Text itemAmount = card.transform.Find("Amount").GetComponent<TMP_Text>();
        TMP_Text itemRarity = card.transform.Find("Rarity").GetComponent<TMP_Text>();

        switch (levelReward.rewardRarity)
        {
            case LevelReward.RewardRarity.COMMON: rarityBorder.color = Color.gray;
            break;
            case LevelReward.RewardRarity.RARE: rarityBorder.color = Color.blue;
            break;
            case LevelReward.RewardRarity.LEGENDARY: rarityBorder.color = Color.yellow;
            break;
            case LevelReward.RewardRarity.SECRET: rarityBorder.color = Color.red;
            break;
        }
        itemName.text = levelReward.nameReward;
        itemDesc.text = levelReward.desc;
        itemAmount.text = $"{levelReward.amount * 100}%";
        itemRarity.text = levelReward.rewardRarity.ToString();

    }

    private void OnLevelUp()
    {
        if(IsLeveling) return;
        
        IsLeveling = true;
        level++;
        neededXP = neededXP * 1.2f;

        InitializeCards();
        uIManager.ActivateLevel();
    }
    public void AddXP(float xp)
    {
        curXP += xp;
        if(curXP >= neededXP)
        {            
            float leftover = neededXP - curXP;
            curXP = leftover;
            OnLevelUp();
        }
    }
    private float RollAmount(LevelReward.RewardType type, LevelReward.RewardRarity rarity)
    {
        foreach (var scaling in rewardScaling)
        {
            if (scaling.type != type) continue;

            Vector2 range = scaling.amountRange[(int)rarity];
            return Mathf.Round(Random.Range(range.x, range.y) * 100f) / 100f;
        }

        Debug.LogWarning($"No scaling configured for {type}");
        return 0f;
    }

    private void InitializeStrings(LevelReward levelReward)
    {
        switch (levelReward.rewardType)
        {
            case LevelReward.RewardType.DMG: levelReward.desc = "A damage multiplier applied to all your weapons."; levelReward.nameReward = "Damage";
            break;
            case LevelReward.RewardType.FIRERATE: levelReward.desc = "A firerate multiplier applied to all your weapons."; levelReward.nameReward = "Firerate";
            break;
            case LevelReward.RewardType.HP: levelReward.desc = "A health multiplier"; levelReward.nameReward = "Health";
            break;
            case LevelReward.RewardType.SPD: levelReward.desc = "Speed multiplier"; levelReward.nameReward = "Speed";
            break;
        }
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
