using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private GameObject scoreObject;

    void Start()
    {
        InitializeScoreboard();
    }

    public void InitializeScoreboard()
    {
        foreach (Transform child in content) Destroy(child.gameObject);
    
        List<GameManager.ScoreData> scoreDatas = GameManager.LoadScores();
        scoreDatas.Sort();

        foreach (var item in scoreDatas)
        {
            GameObject score = Instantiate(scoreObject, content);
            score.transform.Find("Name").GetComponent<TMP_Text>().text = item.playerName;
            score.transform.Find("Score").GetComponent<TMP_Text>().text = item.score.ToString("0");
        }
    }
}
