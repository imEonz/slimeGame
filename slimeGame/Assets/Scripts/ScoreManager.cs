using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private float score;
    [SerializeField]
    private Dictionary<string, float> triggers = new Dictionary<string, float>();
    [SerializeField]
    private GameObject multText;
    private float time;
    [SerializeField]
    TMP_Text timeText;
    [SerializeField]
    TMP_Text scoreText;
    [SerializeField]
    private Transform scoreMultText;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        score = 0;
        time = 0;
    }

    void Update()
    {
        time += Time.deltaTime;
        timeText.text = time.ToString("0");

        score += Time.deltaTime;
        UpdateScore();
    }

    public void AddScore(string name, float score)
    {
        if(!triggers.ContainsKey(name))
        {
            triggers.Add(name,score);
        }

        triggers.TryGetValue(name, out float triggerScore);

        GameObject go = Instantiate(multText,scoreMultText);
        go.GetComponent<TMP_Text>().text = "+ " + name;

        Debug.Log($"Triggered: " + name);

        this.score += triggerScore;
        UpdateScore();
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + score.ToString("0");
    }

    public float GetScore()
    {
        return score;
    }
}
