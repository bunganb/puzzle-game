using Core;
using UnityEngine;
using Managers;
using Data;

namespace View
{
    public class PackingDropHandler : IDropHandler2D
    {
        private readonly PackingPuzzleData _data;
        private readonly LayerMask _frameMask;
        private readonly float _snapDistance;

        public PackingDropHandler(
            PackingPuzzleData data,
            LayerMask frameMask,
            float snapDistance)
        {
            _data = data;
            _frameMask = frameMask;
            _snapDistance = snapDistance;
        }

        public bool TryDrop(
            string pieceId,
            Collider2D pieceCollider,
            Vector3 dropWorldPos,
            out Vector3 snapPos)
        {
            snapPos = dropWorldPos;

            if (pieceCollider == null)
                return false;

            // Ambil frame collider
            Collider2D frameCol = Physics2D.OverlapPoint(dropWorldPos, _frameMask);
            if (frameCol == null)
            {
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
                return false;
            }

            // Shift bounds ke posisi drop
            Bounds pieceBounds = pieceCollider.bounds;
            Vector3 offset = dropWorldPos - pieceBounds.center;
            Bounds shiftedBounds = new Bounds(
                pieceBounds.center + offset,
                pieceBounds.size
            );

            Bounds frameBounds = frameCol.bounds;

            // Cek fully inside
            bool fullyInside =
                frameBounds.Contains(shiftedBounds.min) &&
                frameBounds.Contains(shiftedBounds.max);

            if (!fullyInside)
            {
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
                return false;
            }

            // Cek posisi target
            var pieceData = _data.GetPiece(pieceId);
            if (pieceData == null) return false;

            float dist = Vector3.Distance(
                dropWorldPos,
                pieceData.targetPosition
            );

            // 🔍 DEBUG: Log jarak untuk troubleshooting
            Debug.Log($"Piece: {pieceId} | Distance: {dist:F3} | Snap threshold: {_snapDistance}");

            // ✅ Snap dengan jarak lebih generous
            if (dist <= _snapDistance)
            {
                snapPos = pieceData.targetPosition;
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Correct");
                
                // Visual feedback
                AudioManager.Instance?.placeSound();
                
                Debug.Log($"✅ {pieceId} SNAPPED!");
                return true;
            }

            // Masih di dalam frame tapi posisi salah
            Debug.Log($"❌ {pieceId} too far (need < {_snapDistance})");
            PuzzleManager.Instance.TryPlacePiece(pieceId, "InFrameWrong");
            return true;
        }
    }
}