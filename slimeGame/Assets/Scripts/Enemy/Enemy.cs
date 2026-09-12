using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private string enemyName;
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

    private List<EnemyLoot> lootTable = new List<EnemyLoot>();

    void Start()
    {
        enemyState = State.Alive;
        transform.position = new Vector2(Random.Range(-xRange,xRange), Random.Range(-yRange,yRange));

        originalColor = spriteRenderer.color;
        
    }

    public void MyFixedUpdate()
    {
        if(enemyState == State.Alive)
        {
            enemyBehaviour.Tick(Time.deltaTime);
        }
        else if (enemyState == State.Stagger)
        {
            Stagger();
        }
        else if (enemyState == State.Dead)
        {
            em.enemies.Remove(this);
            Destroy(gameObject);
        }

    }

    public void TakeDamage(float damage)
    {
        curHP -= damage;
        if(curHP <= 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            if(staggerChance > Random.Range(0f,1f)) enemyState = State.Stagger;
            else enemyState = State.Dead;
        }
        enemyHealthBar.UpdateSlider();
    }

    public void Iniitialize(EnemyData enemyData)
    {
        enemyName = enemyData.EnemyName;
        maxHP = enemyData.MaxHP;
        curHP = enemyData.MaxHP;
        damage = enemyData.Damage;
        speed = enemyData.Speed;
        staggerChance = enemyData.StaggerChance;
        staggerTime = enemyData.StaggerTime;
        level = enemyData.Level;
        lootTable = enemyData.LootTable;
        spriteRenderer.sprite = enemyData.EnemySprite;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        var template = enemyData.EnemyBehaviour;
        EnemyBehaviour instance = (EnemyBehaviour)gameObject.AddComponent(template.GetType());
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(template), instance);
        instance.Initialize(this);
        enemyBehaviour = instance;
    }

    private void Stagger()
    {
        staggerTimer += Time.deltaTime;

        float t = Mathf.PingPong(staggerTimer * flashSpeed, 1f);
        spriteRenderer.color = Color.Lerp(originalColor, Color.white, t);

        if(staggerTimer > staggerTime) enemyState = State.Dead;
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
