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
            GameObject enemy = Instantiate(enemyPrefab, enemiesFolder, true);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.em = em;
            em.enemies.Add(enemyScript);
        }
    }
}
