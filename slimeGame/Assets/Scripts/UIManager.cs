using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text timeText;
    [SerializeField]
    TMP_Text scoreText;
    [SerializeField]
    TMP_Text scoreMultText;
    private float score;
    private float time;

    void Update()
    {
        time += Time.deltaTime;
        timeText.text = time.ToString();
    }

    public void AddScore(float add)
    {
        score += add;
        scoreText.text = score.ToString();
    }

    public float GetScore()
    {
        return score;
    }
    public float GetTime()
    {
        return time;
    }
}
