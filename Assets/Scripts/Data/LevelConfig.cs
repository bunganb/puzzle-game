using UnityEngine;
namespace Data
{

    public enum PuzzleType
    {
        PackingShape, 
        DvdSorting,
    }

    [CreateAssetMenu(menuName = "Game/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int chapter;
        public int level;

        public PuzzleType puzzleType;
        public GameObject puzzleAreaPrefab;

        public PackingPuzzleData packingData;
        public DvdSortingData sortingData;
        // public GroupingPuzzleData groupingData;
    }

}