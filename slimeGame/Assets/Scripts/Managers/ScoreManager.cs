using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button saveButton;
    [SerializeField]
    private Button exitButton;



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

        InitializeButtons();
    }

    void Update()
    {
        time += Time.deltaTime;
        timeText.text = time.ToString("0");

        score += Time.deltaTime * 5f;
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

    private void InitializeButtons()
    {
        saveButton.onClick.AddListener(() =>
        {
            string player = inputField.text;
            GameManager.ScoreData data = new GameManager.ScoreData
            {
                playerName = player,
                score = this.score
            };
            GameManager.Instance.SaveScore(data);
        });
    }

}
