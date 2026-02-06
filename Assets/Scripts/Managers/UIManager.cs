using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameUIPanel;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private GameObject[] _panelHistory = new GameObject[10];
    private int _historyIndex = -1;
    private bool _isPaused = false;

    private void Awake()
    {
        // Untuk scene level: tidak DontDestroyOnLoad
        // Untuk MainMenu: bisa DontDestroyOnLoad kalau mau
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Auto-detect scene dan show panel yang sesuai
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == mainMenuSceneName)
        {
            ShowPanel(mainMenuPanel);
        }
        else
        {
            // Level scene - show game UI
            ShowPanel(gameUIPanel);
        }
    }

    private void Update()
    {
        // ESC untuk pause/unpause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // ========== PANEL MANAGEMENT ==========

    private void ShowPanel(GameObject panel)
    {
        // Hide all panels
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameUIPanel != null) gameUIPanel.SetActive(false);

        // Show target panel
        if (panel != null)
        {
            panel.SetActive(true);
            
            // Save to history
            _historyIndex++;
            if (_historyIndex < _panelHistory.Length)
                _panelHistory[_historyIndex] = panel;
        }
    }

    // ========== NAVIGATION FUNCTIONS ==========

    public void GoToMainMenu()
    {
        Debug.Log("Go To Main Menu");
        
        // Kalau sudah di MainMenu scene, show panel
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == mainMenuSceneName)
        {
            ShowPanel(mainMenuPanel);
            ResumeGame(); // Unpause kalau paused
        }
        else
        {
            // Load MainMenu scene
            Time.timeScale = 1f; // Reset time
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    public void GoToSettings()
    {
        Debug.Log("Open Settings");
        ShowPanel(settingsPanel);
        AudioManager.Instance?.placeSound();
    }

    public void GoToCredits()
    {
        Debug.Log("Open Credits");
        ShowPanel(creditsPanel);
        AudioManager.Instance?.placeSound();
    }

    public void GoToGame()
    {
        Debug.Log("Go To Game");
        ShowPanel(gameUIPanel);
        ResumeGame();
    }

    public void GoBack()
    {
        Debug.Log("Go Back");
        
        // Kembali ke panel sebelumnya
        if (_historyIndex > 0)
        {
            _historyIndex--;
            GameObject previousPanel = _panelHistory[_historyIndex];
            
            // Hide current
            if (_historyIndex + 1 < _panelHistory.Length)
            {
                GameObject current = _panelHistory[_historyIndex + 1];
                if (current != null) current.SetActive(false);
            }
            
            // Show previous
            if (previousPanel != null)
                previousPanel.SetActive(true);
        }
        else
        {
            // Default: back to main menu or game
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == mainMenuSceneName)
                ShowPanel(mainMenuPanel);
            else
                ResumeGame();
        }
        
        AudioManager.Instance?.placeSound();
    }

    // ========== PAUSE/RESUME ==========

    public void PauseGame()
    {
        Debug.Log("Pause Game");
        
        _isPaused = true;
        Time.timeScale = 0f; // Freeze game
        
        ShowPanel(pausePanel);
        AudioManager.Instance?.placeSound();
    }

    public void ResumeGame()
    {
        Debug.Log("Resume Game");
        
        _isPaused = false;
        Time.timeScale = 1f; // Unfreeze game
        
        ShowPanel(gameUIPanel);
        AudioManager.Instance?.placeSound();
    }

    // ========== GAME ACTIONS ==========

    public void ResetButton()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        ResumeGame(); // Unpause kalau paused
        GameManager.Instance.ResetCurrentLevel();
        AudioManager.Instance?.placeSound();
    }

    public void NextLevel()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        ResumeGame();
        GameManager.Instance.NextLevel();
        AudioManager.Instance?.placeSound();
    }

    // ========== BELL BUTTON (HINT/HELP) ==========

    public void BellButtonActivate()
    {
        Debug.Log("Bell Button Activated - Show Hint");
        // Implementasi hint/help system
        AudioManager.Instance?.grabSound();
    }

    // ========== UTILITY ==========

    public bool IsPaused()
    {
        return _isPaused;
    }
}