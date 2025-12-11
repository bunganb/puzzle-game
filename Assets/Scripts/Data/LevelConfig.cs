using UnityEngine;
namespace Data
{

    public enum PuzzleType
    {
        PackingShape
    }

    [CreateAssetMenu(menuName = "Game/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int chapter;
        public int level;

        public PuzzleType puzzleType;

        public PackingPuzzleData packingData;
        // public SortingPuzzleData sortingData;
        // public GroupingPuzzleData groupingData;
    }

}