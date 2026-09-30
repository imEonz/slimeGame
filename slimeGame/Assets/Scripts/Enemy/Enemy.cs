using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private string enemyName;
    [SerializeField]
    private float maxHP;
    private float curHP;
    private float damage;
    private int level;
    public Transform player;
    public Rigidbody2D rb;
    private float speed = 5f;
    private float xRange = 25f;
    private float yRange = 12.5f;
    public EnemyManager em;

    [SerializeField, Range(0f,1f)]
    private float staggerChance;
    [SerializeField]
    private float staggerTime;
    private float staggerTimer;
    [SerializeField]
    private EnemyHealthBar enemyHealthBar;

    public enum State {Alive, Dead, Stagger, Consumed}
    public State enemyState;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float flashSpeed = 10f;
    private Color originalColor;
    private EnemyBehaviour enemyBehaviour;
    private List<EnemySkill> enemySkills = new List<EnemySkill>();
    private float timeBetweenCasts;
    private float castTimer;
    private int skillIndex;
    [SerializeField]
    private List<EnemyLoot> lootTable = new List<EnemyLoot>();
    private bool isCasting;

    [SerializeField]
    private GameObject damageNum;
    [SerializeField]
    private GameObject xpDrop;

    void Start()
    {
        enemyState = State.Alive;

        originalColor = spriteRenderer.color;
        
    }

    public void MyFixedUpdate()
    {
        if(enemyState == State.Alive)
        {
            bool blockMovement = TickSkills(Time.deltaTime);
            if(!blockMovement) enemyBehaviour.Tick(Time.deltaTime);
        }
        else if (enemyState == State.Stagger)
        {
            Stagger();
        }
        else if (enemyState == State.Dead)
        {
            em.enemies.Remove(this);
            Instantiate(xpDrop, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

    }

    private bool TickSkills(float deltatime)
    {
        if (enemySkills.Count == 0) return false;

        
        if(isCasting)
        {
            EnemySkill active = enemySkills[skillIndex];
            bool stillcasting = enemySkills[skillIndex].Tick(deltatime);
            if(!stillcasting)
            {
                isCasting = false;
                skillIndex = (skillIndex + 1) % enemySkills.Count;
                castTimer = 0f;
            }
            return active.BlocksMovement;
        }

        castTimer += deltatime;
        if(castTimer >= timeBetweenCasts)
        {
            isCasting = true;
            EnemySkill next = enemySkills[skillIndex];
            next.Begin();
            return next.BlocksMovement;
        }

        return false;
    }

    public void TakeDamage(float damage)
    {
        // float newDamage = damage * em.dmgMult;

        GameObject number = Instantiate(damageNum);
        number.transform.Find("Text").GetComponent<TMP_Text>().text = "-" + damage.ToString("0");
        number.transform.position = this.transform.position;
        Destroy(number,0.5f);

        curHP -= damage;
        if(curHP <= 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            if(staggerChance > Random.Range(0f,1f)) enemyState = State.Stagger;
            else enemyState = State.Dead;
            ScoreManager.Instance.AddScore("Kill",100);
        }
        enemyHealthBar.UpdateSlider();
    }
    public void Heal(float healing)
    {
        curHP = Mathf.Min(curHP + healing, maxHP);
        enemyHealthBar.UpdateSlider();
    }

    public void Iniitialize(EnemyData enemyData, float dm)
    {
        enemyName = enemyData.EnemyName;
        maxHP = enemyData.MaxHP * dm;
        curHP = maxHP;
        damage = enemyData.Damage* dm;
        speed = enemyData.Speed;
        staggerChance = enemyData.StaggerChance;
        staggerTime = enemyData.StaggerTime;
        level = enemyData.Level;
        lootTable = enemyData.LootTable;
        spriteRenderer.sprite = enemyData.EnemySprite;
        timeBetweenCasts = enemyData.TimeBetweenCasts;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        var template = enemyData.EnemyBehaviour;
        EnemyBehaviour instance = (EnemyBehaviour)gameObject.AddComponent(template.GetType());
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(template), instance);
        instance.Initialize(this);
        enemyBehaviour = instance;

        foreach (var item in enemyData.EnemySkills)
        {
            if (item == null) continue;
            EnemySkill enemySkill = (EnemySkill)gameObject.AddComponent(item.GetType());
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(item), enemySkill);
            enemySkill.Initialize(this);
            this.enemySkills.Add(enemySkill);
        }
    }

    private void Stagger()
    {
        staggerTimer += Time.deltaTime;

        float t = Mathf.PingPong(staggerTimer * flashSpeed, 1f);
        spriteRenderer.color = Color.Lerp(originalColor, Color.white, t);

        if(staggerTimer > staggerTime)
        {
            enemyState = State.Dead;
            ScoreManager.Instance.AddScore("Kill",100);
        } 
    }

    public void SetOpacity(float opacity)
    {
        Color c = spriteRenderer.color;
        c.a = opacity;
        spriteRenderer.color = c;
    }

    public List<EnemyLoot> GetEnemyLoot()
    {
        return lootTable;
    }
    public float GetMaxHP()
    {
        return maxHP;
    }
    public float GetCurHP()
    {
        return curHP;
    }
    public float GetDamage()
    {
        return damage;
    }
}
