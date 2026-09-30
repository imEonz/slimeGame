using UnityEngine;

public class EnemyPreSpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float timeUnitilSpawn;
    float timer;

    private EnemyData enemyData;
    private EnemyManager em;
    private float dm;


    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timeUnitilSpawn) 
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    public void InitializePreSpawn(EnemyData enemyData, EnemyManager em, float dm)
    {
        this.enemyData = enemyData;
        this.em = em;
        this.dm = dm;
    }
    public void RandomPosition(Vector2 position)
    {
        transform.position = position;
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.parent);
        enemy.transform.position = transform.position;
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.em = em;
        enemyScript.Iniitialize(enemyData, dm);
        em.enemies.Add(enemyScript);

        Destroy(this.gameObject);
    }
}
