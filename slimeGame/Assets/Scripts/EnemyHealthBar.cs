using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    Enemy attatchedEnemy;
    [SerializeField]
    TMP_Text healthText;
    [SerializeField]
    CanvasGroup canvasGroup;
    [SerializeField]
    Slider hpSlider;
    [SerializeField]
    float inactiveTime;
    float inactiveTimer;

    void Update()
    {
        inactiveTimer += Time.deltaTime;
        if(inactiveTimer > inactiveTime)
        {
            canvasGroup.alpha = math.lerp(canvasGroup.alpha,0f,3f * Time.deltaTime);
        }
    }
    public void UpdateSlider()
    {
        inactiveTimer = 0;
        if(attatchedEnemy.curHP > 0) canvasGroup.alpha = 1f;
        else canvasGroup.alpha = 0;
        healthText.text = $"{attatchedEnemy.curHP}/{attatchedEnemy.maxHP}";
        hpSlider.value = attatchedEnemy.curHP/attatchedEnemy.maxHP;
    }
}
