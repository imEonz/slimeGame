using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyData/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField]
    private string enemyName;
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float damage;
    [SerializeField, Range(0f,1f)]
    private float staggerChance;
    [SerializeField]
    private float staggerTime;
    [SerializeField]
    private int level;
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private List<EnemyLoot> lootTable = new List<EnemyLoot>();
    [SerializeField]
    private EnemyBehaviour enemyBehaviour;
    [SerializeField]
    private List<EnemySkill> enemySkills = new List<EnemySkill>();
    [SerializeField]
    private float timeBetweenCasts;
    [SerializeField]
    private Vector3 scale;


    public string EnemyName => enemyName;
    public float MaxHP => maxHP;
    public float Speed => speed;
    public float Damage => damage;
    public float StaggerChance => staggerChance;
    public float StaggerTime => staggerTime;
    public int Level => level;
    public Vector3 Scale => scale;
    public List<EnemyLoot> LootTable => lootTable;
    public EnemyBehaviour EnemyBehaviour => enemyBehaviour;
    public List<EnemySkill> EnemySkills => enemySkills;
    public float TimeBetweenCasts => timeBetweenCasts;
    public Sprite EnemySprite => sprite;
}