using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyManager em;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float spawnTime;
    private float spawnTimer;
    [SerializeField]
    private int spawnAmount;
    [SerializeField]
    private Transform enemiesFolder;
    [SerializeField]
    private bool isOn;
    [SerializeField]
    private float xRange;
    [SerializeField]
    private float yRange;

    [SerializeField]
    private List<EnemyData> spawnableEnemies;

    private Transform player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }


    void Update()
    {
        if(isOn)
        {
            if(spawnTimer > spawnTime)
            {
                spawnTimer = 0f;
                SpawnEnemies();
            }
            else
            {
                spawnTimer += Time.deltaTime;
            }
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector2 spawnPos;
            int attempts = 0;

            do
            {
                spawnPos = new Vector2(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange));
                attempts++;
            }
            while (Vector2.Distance(spawnPos, player.transform.position) < 5f && attempts < 20);

            GameObject enemy = Instantiate(enemyPrefab, enemiesFolder, true);
            EnemyPreSpawn enemyScript = enemy.GetComponent<EnemyPreSpawn>();
            enemyScript.RandomPosition(spawnPos);
            enemyScript.InitializePreSpawn(spawnableEnemies[Random.Range(0,spawnableEnemies.Count)], em);
        }
    }
}
