using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    void Update()
    {
    }
    public void grabSound()
    {
    }
    public void placeSound()
    {
    }
    public void winSound()
    {
    }
}
