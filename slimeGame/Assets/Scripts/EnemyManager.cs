using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> enemies;
    void FixedUpdate()
    {
        foreach (var enemy in enemies)
        {
            enemy.MyFixedUpdate();
        }
    }
}
