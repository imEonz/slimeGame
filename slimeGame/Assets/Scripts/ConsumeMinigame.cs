using System;
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
        progressSlider.value = 0.35f;
        DimOtherEnemies();
    }

    public void RegisterDashPress()
    {
        if (!root.activeSelf) return;

        progressSlider.value += dashAmount;
        SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.GetAudioClip("slimeStruggle"),transform,0.1f, UnityEngine.Random.Range(0.95f,1f));
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
        progressSlider.value = 0f;
        RestoreEnemies();
        camera.ZoomIn(0);

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
        camera.ZoomIn(progressSlider.value);
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
