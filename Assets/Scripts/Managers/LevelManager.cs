using UnityEngine;
using Data;
namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        private void Start()
        {
            LevelConfig config = GameManager.Instance.CurrentLevel;

            if (config == null)
            {
                Debug.LogError("No LevelConfig loaded!");
                return;
            }

            PuzzleManager.Instance.Initialize(config);
        }
    }

}