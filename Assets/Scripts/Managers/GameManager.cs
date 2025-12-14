using UnityEngine;
using Data;
using Managers;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LevelConfig[] allLevels;
    public LevelConfig CurrentLevel { get; private set; }

    private bool _sceneReady;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartLevel(1, 1);
    }

    public void StartLevel(int chapter, int level)
    {
        CurrentLevel = FindLevel(chapter, level);
        if (CurrentLevel == null)
        {
            Debug.LogError($"Level {chapter}-{level} not found!");
            return;
        }

        // Kalau kamu memang sudah selalu main di scene "Main", cukup init puzzle:
        PuzzleManager.Instance.ClearCurrentPuzzle();
        PuzzleManager.Instance.Initialize(CurrentLevel);
    }

    private LevelConfig FindLevel(int chapter, int level)
    {
        foreach (var config in allLevels)
            if (config.chapter == chapter && config.level == level)
                return config;
        return null;
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
        // nanti: tampilkan UI, tombol Next
        // untuk test cepat:
        NextLevel();
    }
    public void ResetCurrentLevel()
    {
        if (CurrentLevel == null)
        {
            Debug.LogWarning("ResetCurrentLevel called but CurrentLevel is null.");
            return;
        }

        if (PuzzleManager.Instance == null)
        {
            Debug.LogError("PuzzleManager.Instance is null. Pastikan PuzzleManager ada di scene Main.");
            return;
        }

        PuzzleManager.Instance.ClearCurrentPuzzle();
        PuzzleManager.Instance.Initialize(CurrentLevel);
    }
}