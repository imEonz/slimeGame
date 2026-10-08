using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private static string SavePath => Path.Combine(Application.persistentDataPath, "scores.json");
    [Serializable]
    public class ScoreData : IComparable<ScoreData>
    {
        public float score;
        public string playerName;

        public int CompareTo(ScoreData other)
        {
            return other.score.CompareTo(score);
        }
    }

    [Serializable]
    private class ScoreDataList
    {
        public List<ScoreData> scores = new List<ScoreData>();
    }

    private Coroutine sceneCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"{SavePath}");
    }

    public void SaveScore(ScoreData data)
    {
        ScoreDataList list = LoadAll();
        list.scores.Add(data);

        string json = JsonUtility.ToJson(list, true);
        File.WriteAllText(SavePath, json);
        
    }

    public static List<ScoreData> LoadScores()
    {
        return LoadAll().scores;
    }

    private static ScoreDataList LoadAll()
    {
        if (!File.Exists(SavePath)) return new ScoreDataList();

        string json = File.ReadAllText(SavePath);
        ScoreDataList list = JsonUtility.FromJson<ScoreDataList>(json);
        return list ?? new ScoreDataList();
    }

    public void ButtonChangeScene(int scenenum)
    {
        if(sceneCoroutine != null) return;
        sceneCoroutine = StartCoroutine(ChangeSceneCoroutine(scenenum));
    }

    private IEnumerator ChangeSceneCoroutine(int scenenum)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scenenum);
    }
    public void ButtonQuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
