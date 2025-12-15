using Core;
using Data;
namespace Managers
{
    public static class PuzzleRuleFactory
    {
        public static IPuzzleRule Create(LevelConfig config)
        {
            switch (config.puzzleType)
            {
                case PuzzleType.PackingShape:
                    return new PackingPuzzleRule(config.packingData);
                case PuzzleType.DvdSorting:
                    return new DvdSortingRule(config.sortingData);
                default:
                    UnityEngine.Debug.LogError("PuzzleType belum dibuat!");
                    return null;
            }
        }
    }

}