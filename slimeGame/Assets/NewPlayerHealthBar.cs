using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewPlayerHealthBar : MonoBehaviour
{
    private PlayerController pc;
    [SerializeField]
    TMP_Text hpText;
    [SerializeField]
    private Image hpSlider;
    [SerializeField]
    private Transform heart;
    [SerializeField]
    private GameObject max;

    private float beatTimer;
    private float timer;
    private Coroutine heartRoutine;
    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        hpSlider.fillAmount = 1;
        hpText.text = $"{pc.GetPlayerHP().ToString("0")}";

        UpdatePlayerHPBar();
    }

    public void Init()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        hpSlider.fillAmount = 1;
        hpText.text = $"{pc.GetPlayerHP().ToString("0")}";

        UpdatePlayerHPBar();
    }

    public void UpdatePlayerHPBar()
    {
        hpSlider.fillAmount = pc.GetPlayerHP()/pc.GetPlayerMaxHP();
        hpText.text = $"{pc.GetPlayerHP().ToString("0")}";

        if(pc.GetPlayerHP() == pc.GetPlayerMaxHP()) max.SetActive(true);
        else max.SetActive(false);
    }

    void Update()
    {
        beatTimer = 60/(pc.GetPlayerMaxHP()/pc.GetPlayerHP())/60;

        timer += Time.deltaTime;

        if(timer >= beatTimer)
        {
            BeatHeart();
        }
    }

    private void BeatHeart()
    {
        if(heartRoutine != null) return;
        heartRoutine = StartCoroutine(HeartCoroutine());
    }

    private IEnumerator HeartCoroutine()
    {
        Vector3 baseScale = Vector3.one;
        Vector3 peakScale = baseScale * 1.1f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / 0.1f;
            heart.localScale = Vector3.Lerp(baseScale, peakScale, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            heart.localScale = Vector3.Lerp(peakScale, baseScale, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        heart.localScale = baseScale;
        timer = 0f;
        heartRoutine = null;
    }
}
