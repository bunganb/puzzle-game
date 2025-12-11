using UnityEngine;
using Core;
using Data;
using View;
namespace Managers
{
    public class PuzzleManager : MonoBehaviour
    {
        public static PuzzleManager Instance { get; private set; }

        private IPuzzleRule _rule;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize(LevelConfig config)
        {
            _rule = PuzzleRuleFactory.Create(config);

            if (config.puzzleType == PuzzleType.PackingShape)
            {
                SpawnPackingPieces(config.packingData);
            }
        }
        private void SpawnPackingPieces(PackingPuzzleData data)
        {
            foreach (var pieceData in data.pieces)
            {
                GameObject obj = Instantiate(
                    pieceData.prefab,
                    pieceData.startPosition,
                    Quaternion.identity);

                var draggable = obj.GetComponent<DraggablePiece2D>();
                draggable.Setup(pieceData.pieceId, pieceData.targetPosition);
            }
        }

        public bool TryPlacePiece(string pieceId, string state)
        {
            bool ok = _rule.TryPlacePiece(pieceId, state);

            if (ok && _rule.IsCompleted)
                GameManager.Instance.NotifyLevelCompleted();

            return ok;
        }
    }
}