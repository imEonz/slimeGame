using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumeMinigame : MonoBehaviour
{
    [SerializeField] 
    private GameObject root;
    [SerializeField] 
    private Slider progressSlider;
    [SerializeField]
    private float timeToFail;
    [SerializeField]
    private float dashAmount;
    
    private float consumeFigherScale;
    [SerializeField]
    EnemyManager enemyManager;
    [SerializeField]
    CameraFollow camera;
    Enemy targetEnemy;
    private Action<bool> onComplete;

    void Start()
    {
        consumeFigherScale = 1/timeToFail;
    }

    public void StartConsuming(Enemy enemy, Action<bool> onCompleteCallback)
    {
        Time.timeScale = 0f;
        targetEnemy = enemy;
        onComplete = onCompleteCallback;
        root.gameObject.SetActive(true);
        progressSlider.value = 0.5f;
        DimOtherEnemies();
    }

    public void RegisterDashPress()
    {
        if (!root.activeSelf) return;

        progressSlider.value += dashAmount;
        camera.Shake();

        if (progressSlider.value >= 1f)
        {
            EndConsuming(true);
        }
        if (progressSlider.value <= 0f)
        {
            EndConsuming(false);
        }
    }

    private void EndConsuming(bool won)
    {   
        progressSlider.value = 0.5f;
        RestoreEnemies();

        Time.timeScale = 1f;
        root.gameObject.SetActive(false);

        Action<bool> callback = onComplete;
        onComplete = null;
        targetEnemy = null;
        callback?.Invoke(won);

 
    }
    void Update()
    {
        progressSlider.value -= Time.unscaledDeltaTime * consumeFigherScale;
    }

    private void DimOtherEnemies()
    {
        foreach (var enemy in enemyManager.enemies)
        {
            if (enemy == null || enemy == targetEnemy) continue;
            enemy.SetOpacity(0.3f);
        }
    }

    private void RestoreEnemies()
    {
        foreach (var enemy in enemyManager.enemies)
        {
            if (enemy == null || enemy == targetEnemy) continue;
            enemy.SetOpacity(1f);
        }
    }
}
