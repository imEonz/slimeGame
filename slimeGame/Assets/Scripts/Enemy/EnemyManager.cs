using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> enemies;
    private int enemiesKilled;
    private float playerDamageDealt;
    // public float dmgMult;
    void FixedUpdate()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];
            if (enemy == null)
            {
                enemies.RemoveAt(i);
                enemiesKilled++;
                continue;
            }
            enemy.MyFixedUpdate();
        }
    }

    public int GetEnemiesKilled()
    {
        return enemiesKilled;
    }
    public void AddDamage(float dmg)
    {
        playerDamageDealt += dmg;
    }
    public float GetPlayerDamage()
    {
        return playerDamageDealt;
    }
}
