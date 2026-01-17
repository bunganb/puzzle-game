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
        private GameObject _currentArea;
        private Transform _piecesParent;
        private Collider2D _frameCollider;
        private LayerMask _frameMask;
        private LayerMask _slotMask;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Initialize(LevelConfig config)
        {
            ClearCurrentPuzzle();
            SpawnPuzzleArea(config.puzzleAreaPrefab);

            _rule = PuzzleRuleFactory.Create(config);

            IDropHandler2D handler = null;

            switch (config.puzzleType)
            {
                case PuzzleType.PackingShape:
                    handler = new PackingDropHandler(
                        config.packingData,
                        _frameMask,
                        1.5f  
                    );
                    SpawnPackingPieces(config.packingData, handler);
                    break;

                case PuzzleType.DvdSorting:
                    handler = new DvdSlotDropHandler(_slotMask);
                    SpawnDvdPieces(config.sortingData, handler);
                    break;
            }
        }

        private void SpawnDvdPieces(DvdSortingData data, IDropHandler2D handler)
        {
            foreach (var piece in data.pieces)
            {
                GameObject obj = Instantiate(
                    piece.prefab,
                    piece.startPosition,
                    Quaternion.identity,
                    _piecesParent
                );

                var drag = obj.GetComponent<DraggablePiece2D>();
                if (drag != null)
                {
                    drag.Setup(piece.pieceId, handler);
                }
                else
                {
                    Debug.LogError($"Piece {piece.pieceId} tidak punya DraggablePiece2D component!");
                }
            }
        }

        private void SpawnPackingPieces(PackingPuzzleData data, IDropHandler2D handler)
        {
            foreach (var p in data.pieces)
            {
                GameObject obj = Instantiate(
                    p.prefab, 
                    p.startPosition, 
                    Quaternion.identity, 
                    _piecesParent
                );
                
                var drag = obj.GetComponent<DraggablePiece2D>();
                if (drag != null)
                {
                    drag.Setup(p.pieceId, handler);
                }
                else
                {
                    Debug.LogError($"Piece {p.pieceId} tidak punya DraggablePiece2D component!");
                }
            }
        }

        private void SpawnPuzzleArea(GameObject areaPrefab)
        {
            if (areaPrefab == null)
            {
                Debug.LogError("areaPrefab is null! Check LevelConfig.");
                return;
            }

            _currentArea = Instantiate(areaPrefab);
            _currentArea.name = areaPrefab.name;

            _frameMask = LayerMask.GetMask("FrameArea");
            _slotMask = LayerMask.GetMask("DvdSlot");

            // Cari FrameArea dengan trim untuk handle spasi
            Transform frameAreaTransform = null;
            for (int i = 0; i < _currentArea.transform.childCount; i++)
            {
                Transform child = _currentArea.transform.GetChild(i);
                if (child.name.Trim() == "FrameArea")
                {
                    frameAreaTransform = child;
                    break;
                }
            }

            if (frameAreaTransform != null)
            {
                _frameCollider = frameAreaTransform.GetComponent<Collider2D>();
                if (_frameCollider == null)
                {
                    Debug.LogWarning("FrameArea ditemukan tapi tidak ada Collider2D!");
                }
            }
            else
            {
                // Fallback: cari recursive
                frameAreaTransform = FindDeepChild(_currentArea.transform, "FrameArea");
                if (frameAreaTransform != null)
                {
                    _frameCollider = frameAreaTransform.GetComponent<Collider2D>();
                }
                else
                {
                    Debug.LogError("GameObject 'FrameArea' tidak ditemukan di prefab!");
                }
            }

            // Cari atau buat PiecesRoot
            Transform piecesRoot = _currentArea.transform.Find("PiecesRoot");
            if (piecesRoot == null)
            {
                GameObject piecesObj = new GameObject("PiecesRoot");
                piecesObj.transform.SetParent(_currentArea.transform);
                piecesObj.transform.localPosition = Vector3.zero;
                _piecesParent = piecesObj.transform;
            }
            else
            {
                _piecesParent = piecesRoot;
            }

            if (_frameMask == 0)
                Debug.LogWarning("Layer 'FrameArea' tidak ditemukan!");
            if (_slotMask == 0)
                Debug.LogWarning("Layer 'DvdSlot' tidak ditemukan!");
        }

        private Transform FindDeepChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name.Trim() == childName)
                    return child;
                
                Transform result = FindDeepChild(child, childName);
                if (result != null)
                    return result;
            }
            return null;
        }

        public bool TryPlacePiece(string pieceId, string state)
        {
            if (_rule == null)
            {
                Debug.LogError("Rule belum di-initialize!");
                return false;
            }

            bool ok = _rule.TryPlacePiece(pieceId, state);

            if (ok && _rule.IsCompleted)
            {
                AudioManager.Instance?.winSound();
                GameManager.Instance.NotifyLevelCompleted();
            }

            return ok;
        }

        public void ClearCurrentPuzzle()
        {
            if (_currentArea != null)
            {
                Destroy(_currentArea);
                _currentArea = null;
            }

            _piecesParent = null;
            _frameCollider = null;
            _rule = null;
        }
    }
}