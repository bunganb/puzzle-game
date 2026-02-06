using UnityEngine;
using UnityEngine.SceneManagement;
using Data;
using Managers;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Setup")]
    public string mainMenuScene = "MainMenu";
    public string[] levelScenes = { "Level_1_1", "Level_1_2", "Level_1_3", "Level_2_1" }; // Sesuaikan nama scene

    [Header("Level Configs")]
    public LevelConfig[] allLevels;
    public LevelConfig CurrentLevel { get; private set; }

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

    private void Start()
    {
        // Jangan auto start kalau ada main menu
        // StartLevel(1, 1);
    }

    public void StartLevel(int chapter, int level)
    {
        CurrentLevel = FindLevel(chapter, level);
        if (CurrentLevel == null)
        {
            Debug.LogError($"Level {chapter}-{level} not found!");
            return;
        }

        // Load scene untuk level ini
        string sceneName = GetSceneName(chapter, level);
        StartCoroutine(LoadLevelScene(sceneName));
    }

    private IEnumerator LoadLevelScene(string sceneName)
    {
        // Load scene async
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Tunggu sampai scene loaded
        while (!asyncLoad.isDone)
        {
            // Optional: update loading bar
            // LoadingUI.Instance?.SetProgress(asyncLoad.progress);
            yield return null;
        }

        // Scene sudah loaded, tunggu 1 frame untuk semua Awake() selesai
        yield return null;

        // Initialize puzzle di scene baru
        if (PuzzleManager.Instance != null)
        {
            PuzzleManager.Instance.Initialize(CurrentLevel);
        }
        else
        {
            Debug.LogError("PuzzleManager not found in scene! Pastikan ada PuzzleManager di setiap level scene.");
        }
    }

    private string GetSceneName(int chapter, int level)
    {
        // Konversi chapter-level ke nama scene
        // Contoh: (1,1) -> "Level_1_1", (2,1) -> "Level_2_1"
        return $"Level_{chapter}_{level}";
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
        
        // Tampilkan UI Win (optional)
        // WinUI.Instance?.Show(() => NextLevel());
        
        // Atau langsung next level setelah delay
        StartCoroutine(NextLevelWithDelay(2f));
    }

    private IEnumerator NextLevelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
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
            Debug.LogError("PuzzleManager.Instance is null.");
            return;
        }

        PuzzleManager.Instance.ClearCurrentPuzzle();
        PuzzleManager.Instance.Initialize(CurrentLevel);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    // Helper untuk start dari main menu
    public void StartFromMenu(int chapter, int level)
    {
        StartLevel(chapter, level);
    }
}