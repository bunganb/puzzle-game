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

            Bounds pieceBounds = pieceCollider.bounds;
            Vector3 pieceCenter = pieceBounds.center;

            Collider2D frameCol = Physics2D.OverlapPoint(pieceCenter, _frameMask);
            if (frameCol == null)
            {
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
                return false;
            }

            Bounds frameBounds = frameCol.bounds;

            bool fullyInside =
                frameBounds.Contains(pieceBounds.min) &&
                frameBounds.Contains(pieceBounds.max);

            if (!fullyInside)
            {
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
                return false;
            }

            var pieceData = _data.GetPiece(pieceId);
            if (pieceData == null) return false;

            Vector3 targetPos = new Vector3(
                pieceData.targetPosition.x,
                pieceData.targetPosition.y,
                0
            );

            float dist = Vector3.Distance(pieceCenter, targetPos);

            if (dist <= _snapDistance)
            {
                snapPos = targetPos;
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Correct");
                AudioManager.Instance?.placeSound();
                return true;
            }

            PuzzleManager.Instance.TryPlacePiece(pieceId, "InFrameWrong");
            return false;
        }

    }
}