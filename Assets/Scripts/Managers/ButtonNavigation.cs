using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonNavigation : MonoBehaviour
{
    public enum ButtonTargets
    {
        MainMenu,
        Settings,
        Credits,
        Quit,
        Pause,
        Resume,
        InGame,
        Back,
        Reset,
        NextLevel,
        BellButton,
        StartGame
    }

    [Header("Button Settings")]
    [SerializeField] private ButtonTargets buttonTarget;
    
    [Header("Start Game Settings (only for StartGame target)")]
    [SerializeField] private int startChapter = 1;
    [SerializeField] private int startLevel = 1;

    [Header("Audio (optional)")]
    [SerializeField] private bool playClickSound = true;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        
        if (_button != null)
        {
            // Auto-assign onClick listener
            _button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError($"Button component not found on {gameObject.name}!");
        }
    }

    public void OnButtonClicked()
    {
        // Play click sound
        if (playClickSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.placeSound();
        }

        // Execute action based on target
        switch (buttonTarget)
        {
            case ButtonTargets.MainMenu:
                if (UIManager.Instance != null)
                    UIManager.Instance.GoToMainMenu();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.Credits:
                if (UIManager.Instance != null)
                    UIManager.Instance.GoToCredits();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.InGame:
                if (UIManager.Instance != null)
                    UIManager.Instance.GoToGame();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.Settings:
                if (UIManager.Instance != null)
                    UIManager.Instance.GoToSettings();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.Quit:
                QuitGame();
                break;

            case ButtonTargets.Back:
                if (UIManager.Instance != null)
                    UIManager.Instance.GoBack();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.Resume:
                if (UIManager.Instance != null)
                    UIManager.Instance.ResumeGame();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.Pause:
                if (UIManager.Instance != null)
                    UIManager.Instance.PauseGame();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.Reset:
                if (UIManager.Instance != null)
                    UIManager.Instance.ResetButton();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.NextLevel:
                if (UIManager.Instance != null)
                    UIManager.Instance.NextLevel();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.BellButton:
                if (UIManager.Instance != null)
                    UIManager.Instance.BellButtonActivate();
                else
                    Debug.LogError("UIManager.Instance is NULL!");
                break;

            case ButtonTargets.StartGame:
                StartGame();
                break;
        }
    }

    private void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartLevel(startChapter, startLevel);
        }
        else
        {
            Debug.LogError("GameManager.Instance is NULL!");
        }
    }

    private void QuitGame()
    {
        Debug.Log("Quit Game");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // ========== HELPER UNTUK DEBUG ==========
    
    private void OnValidate()
    {
        // Auto-rename GameObject sesuai target
        if (Application.isPlaying) return;
        
        if (_button == null)
            _button = GetComponent<Button>();

        // Update nama GameObject supaya jelas (optional)
        // gameObject.name = $"Button_{buttonTarget}";
    }
}