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

            // Ambil frame collider dari center piece (bukan dari dropWorldPos)
            Bounds pieceBounds = pieceCollider.bounds;
            Vector3 pieceCenter = pieceBounds.center;
            
            Collider2D frameCol = Physics2D.OverlapPoint(pieceCenter, _frameMask);
            if (frameCol == null)
            {
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
                return false;
            }

            Bounds frameBounds = frameCol.bounds;

            // Cek fully inside
            bool fullyInside =
                frameBounds.Contains(pieceBounds.min) &&
                frameBounds.Contains(pieceBounds.max);

            if (!fullyInside)
            {
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
                return false;
            }

            // Cek posisi target
            var pieceData = _data.GetPiece(pieceId);
            if (pieceData == null) return false;

            // Convert Vector2 ke Vector3
            Vector3 targetPos = new Vector3(
                pieceData.targetPosition.x,
                pieceData.targetPosition.y,
                0
            );

            // ✅ Hitung jarak dari CENTER piece (bounds.center) ke target
            float dist = Vector3.Distance(pieceCenter, targetPos);

            // Snap dengan jarak generous
            if (dist <= _snapDistance)
            {
                // ✅ Hitung offset dari center ke pivot
                Vector3 centerToPivot = dropWorldPos - pieceCenter;
                
                // Snap position = target + offset ke pivot
                Vector3 proposedSnapPos = targetPos + centerToPivot;
                
                // 🔍 DEBUG
                Debug.Log($"=== {pieceId} SNAP DEBUG ===");
                Debug.Log($"Current Pivot: {dropWorldPos}");
                Debug.Log($"Current Center: {pieceCenter}");
                Debug.Log($"Target Center: {targetPos}");
                Debug.Log($"Center to Pivot offset: {centerToPivot}");
                Debug.Log($"Proposed Snap Pos (Pivot): {proposedSnapPos}");
                
                // ✅ VALIDASI: Cek apakah shape akan fully inside setelah snap
                // Hitung bounds kalau shape ada di snap position
                Vector3 snapOffset = proposedSnapPos - dropWorldPos;
                Bounds snappedBounds = new Bounds(
                    pieceBounds.center + snapOffset,
                    pieceBounds.size
                );
                
                Debug.Log($"Snapped Bounds: center={snappedBounds.center}, min={snappedBounds.min}, max={snappedBounds.max}");
                Debug.Log($"Frame Bounds: min={frameBounds.min}, max={frameBounds.max}");
                
                // Pastikan setelah snap masih fully inside frame
                bool snapWillBeInside = 
                    frameBounds.Contains(snappedBounds.min) &&
                    frameBounds.Contains(snappedBounds.max);
                
                Debug.Log($"Snap will be inside: {snapWillBeInside}");
                
                if (snapWillBeInside)
                {
                    snapPos = proposedSnapPos;
                    PuzzleManager.Instance.TryPlacePiece(pieceId, "Correct");
                    AudioManager.Instance?.placeSound();
                    return true;
                }
                else
                {
                    // Terlalu dekat tapi snap akan keluar frame
                    Debug.LogWarning($"{pieceId} dekat target tapi snap akan keluar frame!");
                    PuzzleManager.Instance.TryPlacePiece(pieceId, "InFrameWrong");
                    return true;
                }
            }

            // Masih di dalam frame tapi posisi salah
            PuzzleManager.Instance.TryPlacePiece(pieceId, "InFrameWrong");
            return true;
        }
    }
}