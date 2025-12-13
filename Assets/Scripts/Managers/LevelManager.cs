using UnityEngine;
using Data;
namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        private void Start()
        {
            if (GameManager.Instance.CurrentLevel != null)
                PuzzleManager.Instance.Initialize(GameManager.Instance.CurrentLevel);
        }
    }

}