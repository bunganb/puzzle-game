using UnityEngine;
using Core;
using Data;
using View;
namespace Managers
{
    public class PuzzleManager : MonoBehaviour
    {
        public static PuzzleManager Instance { get; private set; }
        [SerializeField] private Transform puzzleRoot;
        private IPuzzleRule _rule;
        [SerializeField] private Transform worldRoot; // tempat menaruh puzzle area (misalnya empty di scene)
        private GameObject _currentArea;
        private Transform _piecesRoot;
        private Collider2D _frameCollider; // optional kalau mau cek frame via collider langsung


        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Initialize(LevelConfig config)
        {
            ClearCurrentPuzzle();

            // 1) Spawn area
            if (config.puzzleAreaPrefab != null)
            {
                _currentArea = Instantiate(config.puzzleAreaPrefab, worldRoot);
                _piecesRoot = _currentArea.transform.Find("PiecesRoot");

                // optional: ambil frame collider kalau namanya konsisten
                var frame = _currentArea.transform.Find("FrameArea");
                if (frame != null) _frameCollider = frame.GetComponent<Collider2D>();
            }

            // 2) Buat rule
            _rule = PuzzleRuleFactory.Create(config);

            // 3) Spawn pieces sesuai puzzle type
            if (config.puzzleType == PuzzleType.PackingShape)
                SpawnPackingPieces(config.packingData);
        }

        private void SpawnPackingPieces(PackingPuzzleData data)
        {
            foreach (var pieceData in data.pieces)
            {
                var parent = _piecesRoot != null ? _piecesRoot : worldRoot;

                GameObject obj = Instantiate(
                    pieceData.prefab,
                    pieceData.startPosition,
                    Quaternion.identity,
                    parent
                );

                var draggable = obj.GetComponent<DraggablePiece2D>();
                draggable.Setup(pieceData.pieceId, pieceData.targetPosition);

                // kalau kamu mau: kasih referensi frameCollider biar cek inside frame lebih akurat
                // draggable.SetFrame(_frameCollider);
            }
        }


        public bool TryPlacePiece(string pieceId, string state)
        {
            bool ok = _rule.TryPlacePiece(pieceId, state);

            if (ok && _rule.IsCompleted)
                GameManager.Instance.NotifyLevelCompleted();

            return ok;
        }
        public void ClearCurrentPuzzle()
        {
            if (_currentArea != null)
                Destroy(_currentArea);

            _currentArea = null;
            _piecesRoot = null;
            _frameCollider = null;
            _rule = null;
        }
    }
}