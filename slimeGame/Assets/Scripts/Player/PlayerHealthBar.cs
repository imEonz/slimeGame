using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private PlayerController pc;
    [SerializeField]
    TMP_Text hpText;
    [SerializeField]
    private Slider hpSlider;
    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        hpSlider.value = 1;
        hpText.text = $"{pc.GetPlayerHP().ToString("0")}/{pc.GetPlayerMaxHP().ToString("0")}";
    }

    public void UpdatePlayerHPBar()
    {
        hpSlider.value = pc.GetPlayerHP()/pc.GetPlayerMaxHP();
        hpText.text = $"{pc.GetPlayerHP().ToString("0")}/{pc.GetPlayerMaxHP().ToString("0")}";
    }
}
