using UnityEngine;
using UnityEngine.SceneManagement;
using Data;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LevelConfig[] allLevels;    // Assign in Inspector

    public LevelConfig CurrentLevel { get; private set; }
    private void Start()
    {
        StartLevel(1, 1); // Chapter 1, Level 1
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLevel(int chapter, int level)
    {
        foreach (var config in allLevels)
        {
            if (config.chapter == chapter && config.level == level)
            {
                CurrentLevel = config;
                SceneManager.LoadScene("LevelScene");
                return;
            }
        }

        Debug.LogError($"Level {chapter}-{level} not found!");
    }

    public void NextLevel()
    {
        int newLevel = CurrentLevel.level + 1;
        int newChapter = CurrentLevel.chapter;

        if (newLevel > 3)
        {
            newChapter++;
            newLevel = 1;
        }

        StartLevel(newChapter, newLevel);
    }

    public void NotifyLevelCompleted()
    {
        Debug.Log("Level Complete!");
        // Show UI → Next Level
    }
}