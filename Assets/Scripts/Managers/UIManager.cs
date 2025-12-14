using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void ResetButton()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        GameManager.Instance.ResetCurrentLevel();
    }

    public void GoToSettings()
    {
        Debug.Log("Open Settings");
        // nanti: open settings panel
    }

    public void GoToMainMenu()
    {
        Debug.Log("Go To Main Menu");
        // nanti: SceneManager.LoadScene("MainMenu");
    }

    public void GoToCredit()
    {
        Debug.Log("Go To Credit");
        // nanti: open credit panel
    }
}