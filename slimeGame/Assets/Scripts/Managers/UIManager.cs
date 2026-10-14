
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum UIState {idle, pause, tab, level}
    [SerializeField]
    private UIState uIState;
    [SerializeField]
    private Transform pauseCanvas;
    [SerializeField]
    private Transform tabCanvas;
    [SerializeField]
    private Transform levelCanvas;
    [SerializeField]
    private Transform gameCanvas;

    [SerializeField]
    private Transform weaponHolder;
    [SerializeField]
    private Transform skillHolder;
    [SerializeField]
    private Transform statsHolder;

    private PlayerController player;
    private PlayerLeveler playerLeveler;
    private Slider xpSlider;
    [SerializeField] private Slider xpTabSlider;
    private WeaponManager weaponManager;

    private Transform gunPanel, swordPanel, arrowPanel;

    private Transform gunEntry, swordEntry, arrowEntry;

    private float enemiesKilled, damageDealt, enemiesConsumed, healthHealed;

    private StatTracker stats;
    private bool statsDirty;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text consumedText;
    [SerializeField] private TMP_Text healedText;
    [SerializeField] private TMP_Text missedText;

    void Start()
    {
        stats = StatTracker.Instance;
        stats.Changed += OnStatChanged;
        statsDirty = true;

        player = GameObject.FindAnyObjectByType<PlayerController>();
        playerLeveler = FindAnyObjectByType<PlayerLeveler>();
        weaponManager = FindAnyObjectByType<WeaponManager>();
        xpSlider = gameCanvas.Find("XPSlider").GetComponent<Slider>();

        foreach (Transform child in weaponHolder)
        {
            if (child.name == "Gun")        { gunEntry = child;   gunPanel = child.Find("Panel"); }
            else if (child.name == "Sword") { swordEntry = child; swordPanel = child.Find("Panel"); }
            else if (child.name == "Arrow") { arrowEntry = child; arrowPanel = child.Find("Panel"); }
        }
    }
    void OnDestroy()
    {
        if (stats != null) stats.Changed -= OnStatChanged;
    }

    void Update()
    {
        if(uIState != UIState.idle) return;
    }

    private void ActivatePause()
    {
        if(!pauseCanvas.gameObject.activeSelf)
        {
            pauseCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f;
            uIState = UIState.pause;
        }
        else
        {
            pauseCanvas.gameObject.SetActive(false);
            if(player.GetPlayerState() != PlayerController.State.Consuming) Time.timeScale = 1f;
            uIState = UIState.idle;
        }
    }

    private void ActivateTab()
    {
        if(!tabCanvas.gameObject.activeSelf)
        {
            UpdateTabMenu();
            UpdateStatsPanel();
            tabCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f;
            uIState = UIState.tab;
        }
        else
        {
            tabCanvas.gameObject.SetActive(false);
            if(player.GetPlayerState() != PlayerController.State.Consuming) Time.timeScale = 1f;
            uIState = UIState.idle;
        }
    }
    public void ActivateLevel()
    {
        if(!levelCanvas.gameObject.activeSelf)
        {
            levelCanvas.gameObject.SetActive(true);
            levelCanvas.GetComponent<Animator>().Play("OnLevelUp");
            Time.timeScale = 0f;
            uIState = UIState.level;
        }
        else
        {
            levelCanvas.gameObject.SetActive(false);
            if(player.GetPlayerState() != PlayerController.State.Consuming) Time.timeScale = 1f;
            uIState = UIState.idle;
        }
    }

    public void RegisterEscPress()
    {
        if(uIState != UIState.level && uIState != UIState.tab) ActivatePause();
    }
    public void RegisterTabPress()
    {
        if(uIState != UIState.level && uIState != UIState.pause) ActivateTab();
    }

    public void UpdateXPSlider(float maxXp, float curXP)
    {
        xpSlider.maxValue = maxXp;
        xpSlider.value = curXP;

        xpTabSlider.maxValue = maxXp;
        xpTabSlider.value = curXP;
    }


    private void UpdateTabMenu()
    {
        gunEntry.gameObject.SetActive(false);
        swordEntry.gameObject.SetActive(false);
        arrowEntry.gameObject.SetActive(false);


        foreach (var activeWeapon in weaponManager.GetWeaponList())
        {
            if (activeWeapon.controllers.Count == 0) continue;
            WeaponController wc = activeWeapon.controllers[0];

            switch (activeWeapon.data.RewardName)   // or wc.weaponName, whatever the field is called
            {
                case "Gun":   ShowWeapon(gunEntry, gunPanel, wc, activeWeapon);     break;
                case "Sword": ShowWeapon(swordEntry, swordPanel, wc,activeWeapon); break;
                case "Arrow": ShowWeapon(arrowEntry, arrowPanel, wc,activeWeapon); break;
            }
        }
    }

    private void ShowWeapon(Transform entry, Transform panel, WeaponController wc, ActiveWeapon activeWeapon)
    {
        entry.gameObject.SetActive(true);
        panel.Find("DMG").GetComponentInChildren<TMP_Text>().text = "DMG: "+wc.GetDamage().ToString();
        panel.Find("FR").GetComponentInChildren<TMP_Text>().text = "Firerate: " + wc.GetFireRate().ToString();

        panel.Find("LVL").GetComponent<TMP_Text>().text = "LVL: " + activeWeapon.curLevel;
        panel.Find("XP").GetComponent<TMP_Text>().text = $"{activeWeapon.experience}/{activeWeapon.curLevel+1}";
        panel.Find("XPSlider").GetComponent<Slider>().value = activeWeapon.experience;
        panel.Find("XPSlider").GetComponent<Slider>().maxValue = activeWeapon.curLevel+1;
    }

    private void UpdateStatsPanel()
    {
        statsHolder.Find("HPMult").GetComponent<TMP_Text>().text = "HP Mult:"+playerLeveler.GetHPMult().ToString();
        statsHolder.Find("SPDMult").GetComponent<TMP_Text>().text = "SPD Mult:"+playerLeveler.GetSPDMult().ToString();
        statsHolder.Find("DMGMult").GetComponent<TMP_Text>().text = "DMG Mult:"+playerLeveler.GetDMGMult().ToString();
        statsHolder.Find("FRMult").GetComponent<TMP_Text>().text = "FR Mult:"+playerLeveler.GetFirerateMult().ToString();

        statsHolder.Find("HPmax").GetComponent<TMP_Text>().text = "Max HP: " + player.GetPlayerMaxHP().ToString();
        statsHolder.Find("SPDmax").GetComponent<TMP_Text>().text = "Speed: " + player.GetPlayerSPD().ToString();
    }

    public void LeveledUp()
    {
        gameCanvas.Find("LevelText").GetComponent<TMP_Text>().text = "LVL: " + playerLeveler.GetLevel().ToString();
        statsHolder.Find("LVL").GetComponent<TMP_Text>().text = "LVL: " + playerLeveler.GetLevel().ToString();
    }


    private void OnStatChanged(TriggerType type)
    {
        statsDirty = true;
    }

    void LateUpdate()
    {
        if (!statsDirty) return;
        statsDirty = false;
        RefreshStatsUI();
    }

    private void RefreshStatsUI()
    {
        killsText.text  = "Kills: " + stats.Kills.ToString();
        consumedText.text = "Consumed: " + stats.TotalConsumed.ToString();
        missedText.text = "Missed Consumes: " + stats.GetCount(TriggerType.OnMissConsume);
        damageText.text = "Dealt: " + Mathf.RoundToInt(stats.TotalDamageDealt).ToString();
        healedText.text = "Healed: " + Mathf.RoundToInt(stats.TotalHpHealed).ToString();
        xpText.text = "XP Grabbed: " + Mathf.RoundToInt(stats.TotalXpGrabbed).ToString();
    }

}
